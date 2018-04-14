namespace AutoTagger.Crawler.Standard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using AutoTagger.Contract;
    using AutoTagger.Crawler.Standard.V1;

    public class CrawlingJob : HttpCrawler, ICrawlingJob
    {
        private ExploreTagsPageCrawler exploreTagsPageCrawler;

        public CrawlingJob()
        {
            this.exploreTagsPageCrawler = new ExploreTagsPageCrawler();
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

            var hashTags = document
                .SelectNodes("//meta[@property='instExtractQualityFromDescriptionStringpp:hashtags']")
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

        public IEnumerable<string> GetShortcodesFromHashtag(string hashTag)
        {
            return this.exploreTagsPageCrawler.Parse(hashTag);
            //var url = $"https://www.instagram.com/explore/tags/{hashTag}/";
            //return this.GetShortCodesFromUrl(url);
        }

        //public IEnumerable<string> GetShortCodesFromUrl(string url)
        //{
        //    Console.WriteLine("Processing Url " + url);

        //    var res        = this.HttpClient.GetStringAsync(url).Result;
        //    var matches    = Regex.Matches(res, @"\""shortcode\""\:\""([^\""]+)""");
        //    var shortcodes = matches.OfType<Match>().Select(m => m.Groups[1].Value);

        //    return shortcodes;
        //}

        private (int likes, int comments) ExtractQualityFromDescription(string qualityString)
        {
            var likes    = 0;
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

            var likesString    = qualityMatch.Groups[1].Value;
            var commentsString = qualityMatch.Groups[2].Value;

            int.TryParse(likesString, out likes);
            int.TryParse(commentsString, out comments);

            return (likes, comments);
        }
    }
}
