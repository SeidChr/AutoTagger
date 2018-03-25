using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using AutoTagger.Contract;
using HtmlAgilityPack;

namespace AutoTagger.Crawler.Standard
{
    public class Crawler : ICrawler
    {
        public IEnumerable<ICrawlerImage> GetImages(int amount, string url)
        {
            var initialShortCodes = GetShortCodesFromUrl("https://www.instagram.com/");

            var images = new Dictionary<string, ICrawlerImage>();
            var processedHashTags = new HashSet<string>();
            var hashTagQueue = new ConcurrentQueue<string>();

            foreach (var hashTag in initialShortCodes.Select(GetImageDataFromShortCode).SelectMany(x => x.HumanoidTags).Distinct())
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

                var shortCodes = GetShortCodesFromHashTag(currentHashTag);

                var imageData = shortCodes.Select(GetImageDataFromShortCode).Where(x => x != null);
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

        public IEnumerable<ICrawlerImage> GetImagesFromHashTag(int amount, params string[] hashTags)
        {
            var images = new Dictionary<string, ICrawlerImage>();
            var processedHashTags = new HashSet<string>();
            var hashTagQueue = new ConcurrentQueue<string>();
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

                var shortCodes = GetShortCodesFromHashTag(currentHashTag);
                var imageData = shortCodes.Select(GetImageDataFromShortCode).Where(x=>x!=null);
                foreach (var crawlerImage in imageData)
                {
                    foreach (var tag in crawlerImage.HumanoidTags
                        .Where(t=>!processedHashTags.Contains(t)&&!hashTagQueue.Contains(t)))
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

        public ICrawlerImage GetImageDataFromShortCode(string shortCode)
        {
            Console.WriteLine("Processing ShortCode " + shortCode);
            HttpClient hc = new HttpClient();
            HttpResponseMessage result;
            try
            {
                result = hc.GetAsync($"https://www.instagram.com/p/{shortCode}/").Result;
            }
            catch (Exception)
            {
                Console.WriteLine("Exception while fetching image data for shortcode " + shortCode);
                return null;
            }

            var document = new HtmlDocument();
            document.Load(result.Content.ReadAsStreamAsync().Result);
            var imageUrl = document.DocumentNode.SelectNodes("//meta[@property='og:image']")?.FirstOrDefault()?.Attributes["content"]?.Value;
            var hashTags = document.DocumentNode.SelectNodes("//meta[@property='instapp:hashtags']")?.Select(x => x?.Attributes["content"]?.Value);
            //Console.WriteLine("URL: " + imageUrl);
            //Console.WriteLine("Tags: " + string.Join(", ", hashTags));
            if (hashTags == null)
            {
                return null;
            }

            return new CrawlerImage
            {
                HumanoidTags = hashTags.Where(x=>x!=null),
                ImageId = shortCode,
                ImageUrl = imageUrl,
            };

            /// Bgsth_jAPup
            /// <meta property="og:description" content="Gefällt 46 Mal, 1 Kommentare - Christian Seidlitz (@seidchr) auf Instagram: „silent alster 3 incredible calm alsterwasser and an awesome littelbit of fog in the athmosphere…“" />
            /// <meta property="instapp:hashtags" content="wearehamburg" /><meta property="instapp:hashtags" content="welovehh" /><meta property="instapp:hashtags" content="hambourg" /><meta property="instapp:hashtags" content="iamatraveler" />
        }

        public IEnumerable<string> GetShortCodesFromHashTag(string hashTag)
        {
            return GetShortCodesFromUrl($"https://www.instagram.com/explore/tags/{hashTag}/");
        }

        public IEnumerable<string> GetShortCodesFromUrl(string url)
        {
            Console.WriteLine("Processing Url " + url);

            HttpClient hc = new HttpClient();
            var x = hc.GetStringAsync(url).Result;
            var matches = Regex.Matches(x, @"\""shortcode\""\:\""([^\""]+)""");
            var shortcodes = matches.OfType<Match>().Select(m => m.Groups[1].Value);

            //Console.WriteLine("ShortCodes: " + string.Join(", ", shortcodes));
            return shortcodes;
            /// Bgsth_jAPup
            /// <meta property="og:description" content="Gefällt 46 Mal, 1 Kommentare - Christian Seidlitz (@seidchr) auf Instagram: „silent alster 3 incredible calm alsterwasser and an awesome littelbit of fog in the athmosphere…“" />
            /// <meta property="instapp:hashtags" content="wearehamburg" /><meta property="instapp:hashtags" content="welovehh" /><meta property="instapp:hashtags" content="hambourg" /><meta property="instapp:hashtags" content="iamatraveler" />
        }

        public ICrawlerImage GetCrawlerImageForImageId(string imageId)
        {
            return GetImageDataFromShortCode(imageId);
        }
    }
}
