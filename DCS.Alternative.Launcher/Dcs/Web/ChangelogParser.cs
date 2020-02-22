using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DCS.Alternative.Launcher.Dcs.Web
{
    public class Changelog
    {
        public DateTime DateTime
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }
    }

    public class ChangelogParser
    {
        private readonly string _url;

        public ChangelogParser(string url)
        {
            _url = url;
        }

        public Task<string[]> ParseAsync()
        {
            return Task.Run(async () =>
            {
                using (var client = new HttpClient())
                {
                    var html = await client.GetStringAsync(_url);
                    var doc = new HtmlDocument();

                    doc.LoadHtml(html);

                    var nodes = doc.DocumentNode.SelectNodes($"//*[contains(@class,'changelog')]").ToArray();

                    return await recursiveParseAsync(nodes[0].ChildNodes[1].ChildNodes.ToArray());
                }
            });
        }

        private async Task<string[]> recursiveParseAsync(HtmlNode[] nodes)
        {
            var results = new List<string>();

            foreach (var node in nodes)
            {
                if (node.GetClasses().Any(c=>c.Contains("share")))
                {
                    continue;
                }

                if (node.HasChildNodes)
                {
                    results.AddRange(await recursiveParseAsync(node.ChildNodes.ToArray()));
                    continue;
                }

                var text = node.InnerText;

                if (!string.IsNullOrWhiteSpace(text))
                {
                    text = WebUtility.HtmlDecode(text).Replace("\r\n", " ").Replace("\t", "").TrimEnd('-').Trim();

                    if (node.ParentNode.Name == "h3")
                    {
                        text = $"**{text}**";
                    }
                    else if (node.ParentNode.ParentNode.Name == "li")
                    {
                        text = $"   * {text}";
                    }
                    else
                    {
                        text = $"  {text}";
                    }

                    results.Add(text);
                }

            }

            return results.ToArray();
        }
    }
}
