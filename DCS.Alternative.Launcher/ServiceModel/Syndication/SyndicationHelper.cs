using System.IO;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DCS.Alternative.Launcher.Xml;

namespace DCS.Alternative.Launcher.ServiceModel.Syndication
{
    public static class SyndicationHelper
    {
        public static Task<SyndicationFeed> GetFeedAsync(string url)
        {
            return Task.Run(async () =>
            {
                SyndicationFeed rssFeed;

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);

                    //will throw an exception if not successful
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    content = XmlHelper.RemoveInvalidXmlChars(content);
                    var buffer = Encoding.UTF8.GetBytes(content);

                    using (var stream = new MemoryStream(buffer))
                    using (var reader = XmlReader.Create(stream))
                    {
                        rssFeed = SyndicationFeed.Load(reader);
                    }
                }

                return rssFeed;
            });
        }
    }
}