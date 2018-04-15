﻿namespace AutoTagger.Crawler.Standard.V1
{
    using System.Collections.Generic;
    using AutoTagger.Contract;
    using AutoTagger.Crawler.Standard.V1;
    using AutoTagger.Crawler.Standard.V1.Crawler;

    public class CrawlerV1 : ICrawler
    {
        private readonly HashtagQueue<string> hashtagQueue;

        private readonly RandomTagsCrawler randomTagsCrawler;
        private readonly ExploreTagsCrawler exploreTagsPageCrawler;
        private readonly ImageDetailCrawler imageDetailPageCrawler;
        private readonly UserCrawler userCrawler;

        public CrawlerV1()
        {
            this.hashtagQueue = new HashtagQueue<string>();

            this.randomTagsCrawler = new RandomTagsCrawler();
            this.exploreTagsPageCrawler = new ExploreTagsCrawler();
            this.imageDetailPageCrawler = new ImageDetailCrawler();
            this.userCrawler = new UserCrawler();
        }

        public IEnumerable<IImage> DoCrawling(int limit, params string[] customTags)
        {
            this.BuildTags(customTags);
            this.hashtagQueue.SetLimit(limit);
            return this.hashtagQueue.Process(
                this.ExploreTagsCrawlerFunc,
                this.ImagePageCrawlerFunc,
                this.UserCrawlerFunc
                );
        }

        private void BuildTags(string[] customTags)
        {
            var tags = customTags.Length == 0 ? this.randomTagsCrawler.Parse() : customTags;
            this.hashtagQueue.Build(tags);
        }

        private IEnumerable<string> ExploreTagsCrawlerFunc(string hashTag)
        {
            var url = $"https://www.instagram.com/explore/tags/{hashTag}/";
            var images = this.exploreTagsPageCrawler.Parse(url);
            foreach (var image in images)
            {
                yield return image.Shortcode;
            }
        }

        private string ImagePageCrawlerFunc(string shortcode)
        {
            var url = $"https://www.instagram.com/p/{shortcode}/?hl=en";
            return this.imageDetailPageCrawler.Parse(url);
        }

        private IEnumerable<IImage> UserCrawlerFunc(string user)
        {
            var url = $"https://www.instagram.com/{user}/?hl=en";
            var images = this.userCrawler.Parse(url);
            foreach (var image in images)
            {
                image.User = user;
                yield return image;
            }
        }
    }
}
