using System;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;

namespace AutoTagger.Crawler.Standard
{
    public class Crawler
    {
        public async void Process(string intagramImageId)
        {
            HttpClient hc = new HttpClient();
            HttpResponseMessage result = await hc.GetAsync($"https://www.instagram.com/p/{intagramImageId}/");
            var document = new HtmlDocument();
            document.Load(await result.Content.ReadAsStreamAsync());
            var imageUrl = document.DocumentNode.SelectNodes("//meta[@property='og:image']").FirstOrDefault().Attributes["content"].Value;
            var hashTags = document.DocumentNode.SelectNodes("//meta[@property='instapp:hashtags']").Select(x => x.Attributes["content"].Value);
            Console.WriteLine("URL: " + imageUrl);
            Console.WriteLine("Tags: " + string.Join(", ", hashTags));
            /// Bgsth_jAPup
            /// <meta property="og:description" content="Gefällt 46 Mal, 1 Kommentare - Christian Seidlitz (@seidchr) auf Instagram: „silent alster 3 incredible calm alsterwasser and an awesome littelbit of fog in the athmosphere…“" />
            /// <meta property="instapp:hashtags" content="wearehamburg" /><meta property="instapp:hashtags" content="welovehh" /><meta property="instapp:hashtags" content="hambourg" /><meta property="instapp:hashtags" content="iamatraveler" />
        }
    }
}
