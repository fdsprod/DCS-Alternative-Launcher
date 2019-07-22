using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Models;
using DCS.Alternative.Launcher.Modules;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.ServiceModel.Syndication;
using DCS.Alternative.Launcher.Services;
using HtmlAgilityPack;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Game.Views
{
    public class GameViewModel : NavigationAwareBase
    {
        private readonly IContainer _container;
        private readonly ISettingsService _settingsService;

        public GameViewModel(IContainer container)
        {
            _container = container;
            _settingsService = _container.Resolve<ISettingsService>();

            RepairDcsCommand.Subscribe(OnRepairDcs);
            LaunchDcsCommand.Subscribe(OnLaunchDcs);
            ShowNewsArticleCommand.Subscribe(OnShowNewsArticle);
        }

        public ReactiveProperty<bool> IsLoading
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveCommand RepairDcsCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveCommand LaunchDcsCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveProperty<string> LatestEagleDynamicsYouTubeUrl
        {
            get;
        } = new ReactiveProperty<string>();

        public ReactiveProperty<NewsArticleModel> LatestNewsArticle
        {
            get;
        } = new ReactiveProperty<NewsArticleModel>();

        public ReactiveProperty<NewsArticleModel> PreviousNewsArticle
        {
            get;
        } = new ReactiveProperty<NewsArticleModel>();

        public ReactiveCommand<NewsArticleModel> ShowNewsArticleCommand
        {
            get;
        } = new ReactiveCommand<NewsArticleModel>();

        private void OnShowNewsArticle(NewsArticleModel model)
        {
            Process.Start(model.Url.Value);
        }

        public override async Task ActivateAsync()
        {
#pragma warning disable 4014
            Task.Run(async () =>
#pragma warning restore 4014
            {
                try
                {
                    IsLoading.Value = true;
                    await LoadYouTubeVideoAsync();
                    await LoadNewsAsync();
                }
                catch (Exception e)
                {
                    Tracer.Error(e);
                }
                finally
                {
                    IsLoading.Value = false;
                }
            });

            await base.ActivateAsync();
        }

        private Task LoadYouTubeVideoAsync()
        {
            return Task.Run(async () =>
            {
                var feed = await SyndicationHelper.GetFeedAsync("https://www.youtube.com/feeds/videos.xml?channel_id=UCgJRhtnqA-67pKmQ3A2GsgA");
                var latestFeed = feed.Items.OrderByDescending(i => i.PublishDate).FirstOrDefault();

                if (latestFeed != null)
                {
                    var link = latestFeed.Links[0].Uri.ToString();
                    LatestEagleDynamicsYouTubeUrl.Value = link.Replace("watch?v=", "embed/") + "?autoplay=1&rel=0";
                }
            });
        }

        private Task LoadNewsAsync()
        {
            return Task.Run(async () =>
            {
                var articles = new List<NewsArticleModel>();

                using (var client = new HttpClient())
                {
                    var html = await client.GetStringAsync("https://www.digitalcombatsimulator.com/en/news/");
                    var doc = new HtmlDocument();

                    doc.LoadHtml(html);

                    var nodes = doc.DocumentNode.SelectNodes("//*[contains(@class,'well')]").ToArray();

                    foreach (var node in nodes.Take(2))
                    {
                        if (!node.Id.StartsWith("bx_")) continue;

                        var divs = node.SelectNodes("div");
                        var article = new NewsArticleModel();
                        var dayMonth = divs[0].SelectSingleNode("div[1]/div[1]").InnerText.Trim();
                        var year = divs[0].SelectSingleNode("div[1]/div[2]").InnerText.Trim();
                        var title = divs[0].SelectSingleNode("div[2]/div[1]/h3[1]/a[1]").InnerText.Trim();
                        var summary = divs[0].SelectSingleNode("div[2]/div[2]/div[1]").InnerText.Trim();
                        var url = "https://www.digitalcombatsimulator.com" + divs[0].SelectSingleNode("div[2]/a[1]").Attributes["href"].Value.Trim();

                        article.Title.Value = title;
                        article.Summary.Value = summary;
                        article.Url.Value = url;
                        article.Day.Value = dayMonth;
                        article.Year.Value = year;
                        article.ImageSource.Value = $"/Images/Backgrounds/background ({Convert.ToInt32(article.Day.Value.Substring(0, dayMonth.Length - 3).Trim()) % 20 + 1}).jpg";

                        articles.Add(article);
                    }

                    LatestNewsArticle.Value = articles.FirstOrDefault();
                    PreviousNewsArticle.Value = articles.Skip(1).FirstOrDefault();
                }
            });
        }

        private void OnLaunchDcs()
        {
            var module = new A10Module();
            module.PatchViewports(_settingsService.SelectedInstall);
        }

        private void OnRepairDcs()
        {
            var processInfo = new ProcessStartInfo(_settingsService.SelectedInstall.GetUpdaterPath(), "repair");
            var process = Process.Start(processInfo);

            process?.WaitForExit();
        }
    }
}