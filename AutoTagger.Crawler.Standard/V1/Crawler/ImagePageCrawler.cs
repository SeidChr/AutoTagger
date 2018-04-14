namespace AutoTagger.Crawler.Standard.V1.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using AutoTagger.Contract;
    using AutoTagger.Crawler.Standard.V1;

    public class ImagePageCrawler : HttpCrawler
    {

        public string Parse(string shortcode)
        {
            var instaUrl = $"https://www.instagram.com/p/{shortcode}/?hl=en";

            var document = this.FetchDocument(instaUrl);

            var userUrl = document.SelectNodes("//meta[@al:ios:url='og:image']")?.FirstOrDefault()?.Attributes["content"]?.Value;
            var userName = userUrl.Trim('/').Split('/').Last();

            return userName;
        }
    }
}
