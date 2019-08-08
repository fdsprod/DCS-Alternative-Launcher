﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Models;
using DCS.Alternative.Launcher.Modules;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.ServiceModel.Syndication;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using NLua;

namespace DCS.Alternative.Launcher.Services.Dcs
{
    public class DcsWorldService : IDcsWorldService
    {
        private readonly IContainer _container;
        private readonly ISettingsService _settingsService;

        public DcsWorldService(IContainer container)
        {
            _container = container;
            _settingsService = container.Resolve<ISettingsService>();
        }

        public Task<Module[]> GetInstalledAircraftModulesAsync()
        {
            Tracer.Info("Searching DCS for installed modules.");
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
            return Task.Run(async () =>
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
            {
                var settingsService = _container.Resolve<ISettingsService>();
                var modules = new List<Module>();
                var install = settingsService.SelectedInstall;
                var autoupdateModules = install.Modules;

                if (!install.IsValidInstall)
                {
                    return modules.ToArray();
                }

                var aircraftFolders = Directory.GetDirectories(Path.Combine(install.Directory, "Mods//aircraft"));

                foreach (var folder in aircraftFolders)
                {
                    using (var lua = new Lua())
                    {
                        lua.State.Encoding = Encoding.UTF8;

                        var entryPath = Path.Combine(folder, "entry.lua");

                        lua.DoString(
                            @"function _(s) return s end
                                function _(s) return s end
                                function mount_vfs_liveries_path() end
                                function mount_vfs_texture_path() end
                                function mount_vfs_sound_path() end
                                function mount_vfs_model_path() end
                                function make_view_settings() end
                                function set_manual_path() end
                                function dofile() end
                                function plugin_done() end
                                function make_flyable() end
                                AV8BFM = {}
                                F86FM = {}
                                F5E = {}
                                FA18C = {}
                                F15FM = {}
                                FM = {}
                                M2KFM = {}
                                Mig15FM = {}
                                MIG19PFM = {}
                                SA342FM = {}
                                " + $"__DCS_VERSION__ = \"{install.Version}\"");


                        lua.DoString($"current_mod_path = \"{folder.Replace("\\", "\\\\")}\"");

                        var moduleId = string.Empty;
                        var skinsPath = string.Empty;

                        lua["declare_plugin"] = new Action<string, LuaTable>((id, description) =>
                        {
                            if (description.Keys.OfType<string>().All(k => k != "update_id"))
                            {
                                return;
                            }

                            moduleId = description["update_id"].ToString();
                            skinsPath = ((LuaTable)((LuaTable)description["Skins"])[1])["dir"].ToString();
                        });

                        lua["make_flyable"] = new Action<string, string, LuaTable, string>((displayName, b, c, d) =>
                        {
                            if (!string.IsNullOrEmpty(moduleId) && autoupdateModules.Contains(moduleId) && moduleId != "FC3")
                            {
                                var module = new Module()
                                {
                                    ModuleId = moduleId,
                                    DisplayName = displayName,
                                    LoadingImagePath = Path.Combine(folder, skinsPath, "ME", "loading-window.png"),
                                    MainMenuLogoPath = Path.Combine(folder, skinsPath, "ME", "MainMenulogo.png"),
                                    BaseFolderPath = folder,
                                    IconPath = Path.Combine(folder, skinsPath, "icon.png"),
                                    ViewportPrefix = moduleId.ToString().Replace(" ", "_").Replace("-", "_")
                                };
                                modules.Add(module);
                                Tracer.Info($"Found module {displayName}.");
                            }
                        });

                        try
                        {
                            lua.DoFile(entryPath);
                        }
                        catch (Exception e)
                        {
                            Tracer.Error(e);
                        }
                    }
                }

                return modules.ToArray();
            });
        }

        public Task<ReadOnlyDictionary<string, Version>> GetLatestVersionsAsync()
        {
            return Task.Run(async () =>
            {
                using (var client = new HttpClient())
                {
                    Tracer.Info($"Retrieving latest verions from http://updates.digitalcombatsimulator.com/");

                    var html = await client.GetStringAsync("http://updates.digitalcombatsimulator.com/");
                    var doc = new HtmlDocument();

                    doc.LoadHtml(html);

                    var nodes = doc.DocumentNode.SelectNodes("//*[contains(@class,'well')]").ToArray();
                    var node = nodes.FirstOrDefault();

                    var versions = new Dictionary<string, Version>();

                    if (node != null)
                    {
                        foreach (var h2 in node.SelectNodes("h2"))
                        {
                            var innerText = h2.InnerText;
                            var split = innerText.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                            var version = Version.Parse(split.LastOrDefault() ?? string.Empty);
                            var branch = innerText.ToLower().Contains("stable") ? "stable" : "openbeta";

                            Tracer.Info($"Found {branch} {version}");
                            versions.Add(branch, version);
                        }
                    }

                    return new ReadOnlyDictionary<string, Version>(versions);
                }
            });
        }

        public Task<NewsArticleModel[]> GetLatestNewsArticlesAsync(int count = 10)
        {
            return Task.Run(async () =>
            {
                Tracer.Info($"Retrieving latest news from https://www.digitalcombatsimulator.com/en/news/");

                var articles = new List<NewsArticleModel>();

                using (var client = new HttpClient())
                {
                    var html = await client.GetStringAsync("https://www.digitalcombatsimulator.com/en/news/");
                    var doc = new HtmlDocument();

                    doc.LoadHtml(html);

                    var nodes = doc.DocumentNode.SelectNodes("//*[contains(@class,'well')]").ToArray();

                    foreach (var node in nodes.Take(count))
                    {
                        if (!node.Id.StartsWith("bx_"))
                        {
                            continue;
                        }

                        var divs = node.SelectNodes("div");
                        var article = new NewsArticleModel();
                        var dayMonth = divs[0].SelectSingleNode("div[1]/div[1]").InnerText.Trim();
                        var year = divs[0].SelectSingleNode("div[1]/div[2]").InnerText.Trim();
                        var title = divs[0].SelectSingleNode("div[2]/div[1]/h3[1]/a[1]").InnerText.Trim();
                        var summary = divs[0].SelectSingleNode("div[2]/div[2]/div[1]").InnerText.Trim();
                        var url = "https://www.digitalcombatsimulator.com" +
                                  divs[0].SelectSingleNode("div[2]/a[1]").Attributes["href"].Value.Trim();

                        article.Title.Value = title;
                        article.Summary.Value = summary;
                        article.Url.Value = url;
                        article.Day.Value = dayMonth;
                        article.Year.Value = year;
                        article.ImageSource.Value =
                            $"/Images/Backgrounds/background ({Convert.ToInt32(article.Day.Value.Substring(0, dayMonth.Length - 3).Trim()) % 20 + 1}).jpg";

                        articles.Add(article);

                        Tracer.Info($"Found article {title}");
                    }

                    return articles.ToArray();
                }
            });
        }


        public Task<string> GetLatestYoutubeVideoUrlAsync()
        {
            return GetLatestYouTubeVideoAsync(
                "https://www.youtube.com/feeds/videos.xml?channel_id=UCgJRhtnqA-67pKmQ3A2GsgA",
                "https://www.youtube.com/feeds/videos.xml?channel_id=UCHa9LMylydkT0T3qSzAVrlw");
        }

        public Task<string> GetLatestWagsYoutubeVideoUrlAsync()
        {
            return GetLatestYouTubeVideoAsync();
        }

        private Task<string> GetLatestYouTubeVideoAsync(params string[] youtubeFeedUrls)
        {
            return Task.Run(async () =>
            {
                var syndicationItems = new List<SyndicationItem>();
                var tasks = new List<Task<SyndicationFeed>>();

                foreach (var url in youtubeFeedUrls)
                {
                    Tracer.Info($"Retrieving latest youtube videos from {url}");
                    tasks.Add(SyndicationHelper.GetFeedAsync(url));
                }

                await Task.WhenAll(tasks);

                foreach (var feed in tasks.Select(t => t.Result))
                {
                    syndicationItems.AddRange(feed.Items);
                }

                var ordered = syndicationItems.OrderByDescending(i => i.PublishDate);
                var latestFeed = ordered.FirstOrDefault();

                if (latestFeed == null)
                {
                    return string.Empty;
                }

                var link = latestFeed.Links[0].Uri.ToString();
                var result = link.Replace("watch?v=", "embed/") + "?rel=0&disablekb=1&fs=0&modestbranding=1";

                Tracer.Info($"Found video {link}");

                return result;

            });
        }

        public virtual async Task PatchViewportsAsync()
        {
            var install = _settingsService.SelectedInstall;
            var viewportTemplates = _settingsService.GetViewportTemplates();
            var modules = await GetInstalledAircraftModulesAsync();

            foreach (var template in viewportTemplates)
            {
                var module = modules.FirstOrDefault(m => m.ModuleId == template.ModuleId);

                if (module == null)
                {
                    Tracer.Warn($"Could not patch viewport for module {template.ModuleId} because the module is not installed.");
                    return;
                }

                foreach (var viewport in template.Viewports)
                {
                    if (!install.FileExists(viewport.RelativeInitFilePath))
                    {
                        Tracer.Warn($"Module {template.ModuleId}: Unable to patch viewport(s) [{viewport.ViewportName} in file {viewport.RelativeInitFilePath}.");
                        continue;
                    }

                    var contents = install.ReadAllText(viewport.RelativeInitFilePath);
                    var isChanged = false;

                    if (!contents.Contains("dofile(LockOn_Options.common_script_path..\"ViewportHandling.lua\")"))
                    {
                        Tracer.Info($"Adding ViewportHandling code to {viewport.RelativeInitFilePath}");
                        contents += Environment.NewLine +
                                    "dofile(LockOn_Options.common_script_path..\"ViewportHandling.lua\")" +
                                    Environment.NewLine;
                        isChanged = true;
                    }

                    var originalCode = $"try_find_assigned_viewport(\"{viewport.ViewportName}\")";

                    var code = $"try_find_assigned_viewport(\"{module.ViewportPrefix}_{viewport.ViewportName}\")";

                    if (!contents.Contains(code))
                    {
                        Tracer.Info($"Adding viewport name assignment code to {viewport.RelativeInitFilePath}");

                        if (contents.Contains(originalCode))
                        {
                            contents = contents.Replace(originalCode, code);
                        }
                        else
                        {
                            contents += Environment.NewLine + code + Environment.NewLine;
                        }

                        isChanged = true;
                    }

                    if (isChanged)
                    {
                        Tracer.Info($"Saving {viewport.RelativeInitFilePath}");
                        install.WriteAllText(viewport.RelativeInitFilePath, contents);
                    }
                }
            }
        }

        public Task UpdateAdvancedOptionsAsync()
        {
            var options = _settingsService.GetAdvancedOptions(AdvancedOptions.Graphics);

            var sb = new StringBuilder();

            using (var lua = new Lua())
            {
                sb.AppendLine("options = options or {}");
                sb.AppendLine("options.graphics = options.graphics or {}");

                foreach (var option in options)
                {
                    if (_settingsService.TryGetValue<object>(SettingsCategories.AdvancedOptions, option.Id, out var value))
                    {
                        if (value is JArray)
                        {
                            sb.AppendLine($"{option.Id} = {{ {string.Join(",", ((JArray)value).Select(i => i.ToString()).ToArray())} }}");
                        }
                        else if (value is int[])
                        {
                            sb.AppendLine($"{option.Id} = {{ {string.Join(",", ((int[])value).Select(i=>i.ToString()).ToArray())} }}");
                        }
                        else
                        {
                            sb.AppendLine($"{option.Id} = {value}");
                        }
                    }
                }
            }

            var install = _settingsService.SelectedInstall;

            File.WriteAllText(Path.Combine(install.SavedGamesPath, "Config", "autoexec.cfg"), sb.ToString());

            return Task.FromResult(true);
        }
    }
}