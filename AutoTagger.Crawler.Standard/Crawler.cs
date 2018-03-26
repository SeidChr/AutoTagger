namespace AutoTagger.Crawler.Standard
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;

    using AutoTagger.Contract;

    using HtmlAgilityPack;

    public class Crawler : ICrawler
    {
        private readonly HttpClient httpClient;

        public Crawler()
        {
            this.httpClient = new HttpClient();
        }

        public ICrawlerImage GetCrawlerImageForImageId(string imageId)
        {
            return this.GetImageDataFromShortCode(imageId);
        }

        public ICrawlerImage GetImageDataFromShortCode(string shortCode)
        {
            Console.WriteLine("Processing ShortCode " + shortCode);
            HttpResponseMessage result;
            try
            {
                result = this.httpClient.GetAsync($"https://www.instagram.com/p/{shortCode}/?hl=en").Result;
            }
            catch (Exception)
            {
                Console.WriteLine("Exception while fetching image data for shortcode " + shortCode);
                return null;
            }

            var document = new HtmlDocument();
            document.Load(result.Content.ReadAsStreamAsync().Result);
            var imageUrl = document.DocumentNode.SelectNodes("//meta[@property='og:image']")?.FirstOrDefault()
                ?.Attributes["content"]?.Value;
            var qualityString = document.DocumentNode.SelectNodes("//meta[@property='og:description']")
                ?.FirstOrDefault()?.Attributes["content"]?.Value;

            (int likes, int comments) = this.ExtractQualityFromDescriptionString(qualityString);

            var hashTags = document
                .DocumentNode
                .SelectNodes("//meta[@property='instapp:hashtags']")
                ?.Select(x => x?.Attributes["content"]?.Value)
                .Where(tag => tag != null);

            // <meta property="og:description" content="132 Likes, 1 Comments - ..........">
            return new CrawlerImage
            {
                HumanoidTags = hashTags,
                ImageId      = shortCode,
                ImageUrl     = imageUrl,
                Likes        = likes,
                Comments     = comments
            };
        }

        public IEnumerable<ICrawlerImage> GetImagesFromHashTag(int amount, params string[] hashTags)
        {
            var images            = new Dictionary<string, ICrawlerImage>();
            var processedHashTags = new HashSet<string>();
            var hashTagQueue      = new ConcurrentQueue<string>();
            foreach (var hashTag in hashTags)
            {
                hashTagQueue.Enqueue(hashTag);
            }

            while (hashTagQueue.TryDequeue(out var currentHashTag))
            {
                if (processedHashTags.Contains(currentHashTag))
                {
                    continue;
                }

                processedHashTags.Add(currentHashTag);

                var shortCodes = this.GetShortCodesFromHashTag(currentHashTag);
                var imageData  = shortCodes.Select(this.GetImageDataFromShortCode).Where(x => x != null);
                foreach (var crawlerImage in imageData)
                {
                    foreach (var tag in crawlerImage.HumanoidTags
                        .Where(t => !processedHashTags.Contains(t) && !hashTagQueue.Contains(t)))
                    {
                        hashTagQueue.Enqueue(tag);
                    }

                    if (images.ContainsKey(crawlerImage.ImageId))
                    {
                        continue;
                    }

                    images[crawlerImage.ImageId] = crawlerImage;
                    yield return crawlerImage;
                    if (images.Count >= amount)
                    {
                        yield break;
                    }
                }
            }
        }

        public IEnumerable<string> GetShortCodesFromHashTag(string hashTag)
        {
            return this.GetShortCodesFromUrl($"https://www.instagram.com/explore/tags/{hashTag}/");
        }

        public IEnumerable<string> GetShortCodesFromUrl(string url)
        {
            Console.WriteLine("Processing Url " + url);

            var x          = this.httpClient.GetStringAsync(url).Result;
            var matches    = Regex.Matches(x, @"\""shortcode\""\:\""([^\""]+)""");
            var shortcodes = matches.OfType<Match>().Select(m => m.Groups[1].Value);

            return shortcodes;
        }

        private (int likes, int comments) ExtractQualityFromDescriptionString(string qualityString)
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

            int.TryParse(likesString,    out likes);
            int.TryParse(commentsString, out comments);

            return (likes, comments);
        }
    }
}
