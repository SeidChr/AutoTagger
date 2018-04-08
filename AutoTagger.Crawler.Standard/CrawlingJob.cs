using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Crawler.Standard
{
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;

    using AutoTagger.Contract;

    using HtmlAgilityPack;

    public class CrawlingJob : ICrawlingJob
    {
        private readonly HttpClient httpClient;

        public CrawlingJob()
        {
            this.httpClient   = new HttpClient();
        }

        public IImage GetImageDataFromShortcode(string shortcode)
        {
            Console.WriteLine("Processing ShortCode " + shortcode);

            var instaUrl = $"https://www.instagram.com/p/{shortcode}/?hl=en";

            var document = this.FetchDocument(instaUrl);

            var imageUrl = document.SelectNodes("//meta[@property='og:image']")?.FirstOrDefault()?.Attributes["content"]
                ?.Value;

            var qualityString = document.SelectNodes("//meta[@property='og:description']")?.FirstOrDefault()
                ?.Attributes["content"]?.Value;

            (int likes, int comments) = this.ExtractQualityFromDescription(qualityString);

            var hashTags = document.SelectNodes("//meta[@property='instExtractQualityFromDescriptionStringpp:hashtags']")
                ?.Select(x => x?.Attributes["content"]?.Value).Where(tag => tag != null);

            // <meta property="og:description" content="132 Likes, 1 Comments - ..........">
            var result = new Image
            {
                HumanoidTags = hashTags ?? Enumerable.Empty<string>(),
                ImageId      = shortcode,
                ImageUrl     = imageUrl,
                InstaUrl     = instaUrl,
                Likes        = likes,
                Comments     = comments
            };

            return result;
        }

        public IEnumerable<string> GetRandomHashtags()
        {
            // https://top-hashtags.com/random/
            // https://www.all-hashtag.com/library/contents/ajax_top.php
            var document = this.FetchDocument("https://www.all-hashtag.com/library/contents/ajax_top.php");

            // <section id="tab1" class="tab"><h4 class="tab-title">Top 100 hashtags <span class="color-brand">today</span></h4><span class="hashtag">#look</span>
            var nodes = document.SelectNodes("//section[@id='tab1']//span[@class='hashtag']");
            return nodes.Select(n => n.InnerText.Trim(' ', '#'));
        }

        private (int likes, int comments) ExtractQualityFromDescription(string qualityString)
        {
            var likes = 0;
            var comments = 0;

            if (string.IsNullOrWhiteSpace(qualityString))
            {
                return (likes, comments);
            }

            var qualityMatch = Regex.Match(qualityString, @"(\d+)\sLikes,\s(\d+)\sComments\s-\s");
            if (!qualityMatch.Success)
            {
                return (likes, comments);
            }

            var likesString = qualityMatch.Groups[1].Value;
            var commentsString = qualityMatch.Groups[2].Value;

            int.TryParse(likesString, out likes);
            int.TryParse(commentsString, out comments);

            return (likes, comments);
        }

        private HtmlNode FetchDocument(string url)
        {
            HttpResponseMessage result;
            try
            {
                result = this.httpClient.GetAsync(url).Result;
            }
            catch (Exception)
            {
                Console.WriteLine("Exception while fetching url " + url);
                return null;
            }

            var document = new HtmlDocument();
            document.Load(result.Content.ReadAsStreamAsync().Result);
            return document.DocumentNode;
        }

        public IEnumerable<string> GetShortcodesFromHashtag(string hashTag)
        {
            var url = $"https://www.instagram.com/explore/tags/{hashTag}/";
            return this.GetShortCodesFromInstagramUrl(url);
        }

        public IEnumerable<string> GetShortCodesFromInstagramUrl(string url)
        {
            Console.WriteLine("Processing Url " + url);

            var res = this.httpClient.GetStringAsync(url).Result;
            var matches = Regex.Matches(res, @"\""shortcode\""\:\""([^\""]+)""");
            var shortcodes = matches.OfType<Match>().Select(m => m.Groups[1].Value);

            return shortcodes;
        }

    }
}
