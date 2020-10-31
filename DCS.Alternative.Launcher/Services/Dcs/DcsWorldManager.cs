using System;
using System.Collections;
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
using DCS.Alternative.Launcher.Lua;
using DCS.Alternative.Launcher.Models;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.ServiceModel.Syndication;
using DCS.Alternative.Launcher.Storage.Modules;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLua;

namespace DCS.Alternative.Launcher.Services.Dcs
{
    public class DcsWorldManager : IDcsWorldManager
    {
        private readonly IContainer _container;
        private readonly IProfileService _profileService;
        private readonly Dictionary<string, ModuleBase> _modules = new Dictionary<string, ModuleBase>();

        public DcsWorldManager(IContainer container)
        {
            _container = container;
            _profileService = container.Resolve<IProfileService>();

            var eventRegistry = container.Resolve<ApplicationEventRegistry>();

            eventRegistry.CurrentProfileChanged += OnSelectedProfileChanged;
        }

        private void OnSelectedProfileChanged(object sender, Settings.SelectedProfileChangedEventArgs e)
        {
            Tracer.Info("Profile was changed, clearing module cache.");

            _modules.Clear();
        }

        public Task<KnownModuleDefinition[]> GetKnownModuleDefinitionsAsync(ModuleClassification? classification = null)
        {
            var knownModules = KnownModuleDefinitionStorageAdapter.GetAll();

            return Task.FromResult(knownModules.Where(k=>k.Classification == (classification ?? k.Classification)).ToArray());
        }

        public Task<ModuleBase[]> GetAllModulesAsync()
        {
            var install = _profileService.GetSelectedInstall();

            if (!install.IsValidInstall)
            {
                Tracer.Info("Current install is invalid, aborting...");
                return Task.FromResult(_modules.Values.ToArray());
            }

            return Task.Run(async () =>
            {
                var modules = new List<ModuleBase>();

                modules.AddRange(await GetInstalledAircraftModulesAsync());

                return modules.ToArray();
            });
        }

        public Task<ModuleBase[]> GetInstalledAircraftModulesAsync()
        {
            Tracer.Info("Searching DCS for installed modules.");

            var install = _profileService.GetSelectedInstall();

            if (!install.IsValidInstall)
            {
                Tracer.Info("Current install is invalid, aborting...");
                return Task.FromResult(new ModuleBase[0]);
            }

            return Task.Run( () =>
            {
                using (var context = new AircraftModuleLuaContext(install))
                {
                    return context.GetModules();
                }
            });
        }

        public Task<ReadOnlyDictionary<string, DcsVersion>> GetLatestVersionsAsync()
        {
            return Task.Run(async () =>
            {
                using (var client = new HttpClient())
                {
                    Tracer.Info("Retrieving latest verions from http://updates.digitalcombatsimulator.com/");

                    var html = await client.GetStringAsync("http://updates.digitalcombatsimulator.com/");
                    var doc = new HtmlDocument();

                    doc.LoadHtml(html);

                    var nodes = doc.DocumentNode.SelectNodes("//*[contains(@class,'well')]").ToArray();
                    var node = nodes.FirstOrDefault();

                    var versions = new Dictionary<string, DcsVersion>();

                    if (node != null)
                    {
                        foreach (var h2 in node.SelectNodes("h2"))
                        {
                            var innerText = h2.InnerText;
                            var split = innerText.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                            var version = DcsVersion.Parse(split.LastOrDefault() ?? string.Empty);
                            var branch = innerText.ToLower().Contains("stable") ? "stable" : "openbeta";

                            Tracer.Info($"Found {branch} {version}");
                            versions.Add(branch, version);
                        }
                    }

                    return new ReadOnlyDictionary<string, DcsVersion>(versions);
                }
            });
        }

        public AdditionalResource[] GetAdditionalResourcesByModule(string moduleId)
        {
            var path = "Data\\Resources\\AdditionalResources.json";
            var contents = File.ReadAllText(path);
            var resourceLookup = JsonConvert.DeserializeObject<Dictionary<string, AdditionalResource[]>>(contents);

            path = Path.Combine(ApplicationPaths.ResourcesPath, "AdditionalResources.json");
            contents = File.ReadAllText(path);

            var customResourceLookup = JsonConvert.DeserializeObject<Dictionary<string, AdditionalResource[]>>(contents);
            var resources = new Dictionary<string, AdditionalResource>();

            if (resourceLookup.TryGetValue(moduleId, out var a))
            {
                Array.ForEach(a, v => resources[v.Name] = v);
            }

            if (customResourceLookup.TryGetValue(moduleId, out var b))
            {
                Array.ForEach(b, v => resources[v.Name] = v);
            }

            return resources.Values.ToArray();
        }

        public Task<NewsArticleModel[]> GetLatestNewsArticlesAsync(int count = 10)
        {
            return Task.Run(async () =>
            {
                Tracer.Info("Retrieving latest news from https://www.digitalcombatsimulator.com/en/news/");

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
                        var title = (divs[0].SelectSingleNode("div[2]/div[1]/h3[1]/a[1]"))?.InnerText?.Trim() ?? string.Empty;
                        var summary = divs[0].SelectSingleNode("div[2]/div[2]/div[1]").InnerText.Trim();
                        var url = "https://www.digitalcombatsimulator.com" + ((divs[0].SelectSingleNode("div[2]/a[1]"))?.Attributes["href"]?.Value ?? string.Empty).Trim();

                        article.Title.Value = title;
                        article.Summary.Value = summary;
                        article.Url.Value = url;
                        article.Day.Value = dayMonth;
                        article.Year.Value = year;
                        article.ImageSource.Value = $"/Images/Backgrounds/background ({Convert.ToInt32(article.Day.Value.Substring(0, dayMonth.Length - 3).Trim()) % 20 + 1}).jpg";

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

        public Task WriteOptionsAsync()
        {
            return Task.Run(() =>
            {
                var install = _profileService.GetSelectedInstall();
                var categories = _profileService.GetGameOptions();
                var optionsFile = Path.Combine(install.SavedGamesPath, "Config", "options.lua");

                if (!File.Exists(optionsFile))
                {
                    Tracer.Warn($"options.lua was not found in path {optionsFile}");
                    return Task.FromResult(true);
                }

                string contents = null;

                try
                {
                    contents = File.ReadAllText(optionsFile);
                }
                catch (Exception ex)
                {
                    Tracer.Error(ex);
                    return Task.FromResult(true);
                }

                try
                {
                    using (var context = new OptionLuaContext(install))
                    {
                        foreach (var category in categories)
                        {
                            foreach (var option in category.Options)
                            {
                                if (!_profileService.TryGetValue<object>(ProfileCategories.GameOptions, option.Id, out var value))
                                {
                                    continue;
                                }

                                context.SetValue(category.Id, option.Id, value);
                            }
                        }

                        context.Save();
                    }
                }
                catch (Exception ex)
                {
                    Tracer.Error(ex);

                    if (!string.IsNullOrEmpty(contents))
                    {
                        File.WriteAllText(optionsFile, contents);
                    }
                }

                return Task.FromResult(true);
            });
        }

        public Task UpdateAdvancedOptionsAsync()
        {
            return Task.Run(() =>
            {
                var install = _profileService.GetSelectedInstall();

                File.WriteAllText(install.AutoexecCfg, "");

                using (var context = new AutoexecLuaContext(install))
                {
                    WriteOptions(OptionCategory.General, context);
                    WriteOptions(OptionCategory.Graphics, context);
                    WriteRangedOptions(OptionCategory.Camera, context);
                    WriteRangedOptions(OptionCategory.CameraMirrors, context);
                    WriteOptions(OptionCategory.Terrain, context);
                    WriteOptions(OptionCategory.TerrainMirror, context);
                    WriteOptions(OptionCategory.TerrainReflection, context);
                    WriteOptions(OptionCategory.Sound, context);
                }
            });
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

        private void WriteOptions(string category, AutoexecLuaContext context)
        {
            var options = _profileService.GetAdvancedOptions(category);

            foreach (var option in options)
            {
                if (!_profileService.TryGetValue<object>(ProfileCategories.AdvancedOptions, option.Id, out var value))
                {
                    continue;
                }

                context.SetValue(option.Id, value);
                context.Save(option.Id);
            }
        }

        private void WriteRangedOptions(string category, AutoexecLuaContext context)
        {
            var options = _profileService.GetAdvancedOptions(category);

            foreach (var range in CameraRangeSettings.All)
            {
                foreach (var option in options)
                {
                    if (!_profileService.TryGetValue<object>(ProfileCategories.AdvancedOptions, option.Id, out var value))
                    {
                        continue;
                    }

                    context.SetValue(option.Id.Replace("Extreme", range), value);
                    context.Save(option.Id.Replace("Extreme", range));
                }
            }
        }

        private void WriteOptionValue(StringBuilder sb, string id, object value)
        {
            if (!(value is string) && value is IEnumerable)
            {
                var enumerable = (IEnumerable) value;
                var values =
                    (value is JArray
                        ? enumerable.OfType<JValue>().Select(j => j.Value)
                        : enumerable)
                    .Cast<object>()
                    .Select(Convert.ToDouble)
                    .ToArray();

                sb.AppendLine($"{id} = {{ {string.Join(",", values.Select(i => i.ToString()).ToArray())} }}");
            }
            else
            {
                var valueStr = value is bool ? value.ToString().ToLower() : value.ToString();
                sb.AppendLine($"{id} = {valueStr}");
            }
        }
    }
}