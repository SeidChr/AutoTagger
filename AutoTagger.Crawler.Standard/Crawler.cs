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
        private readonly HashtagQueue<string> hashtagQueue;
        private readonly CrawlingJob crawlingJob;

        public Crawler()
        {
            this.hashtagQueue = new HashtagQueue<string>();
            this.crawlingJob = new CrawlingJob();
        }

        public IEnumerable<IImage> DoCrawling(int limit, params string[] customTags)
        {
            this.BuildQueue(customTags);
            this.hashtagQueue.SetLimit(limit);
            return this.hashtagQueue.Start(this.GetShortCodes, this.Crawl);
        }

        private void BuildQueue(string[] customTags)
        {
            if (customTags.Length == 0)
            {
                this.hashtagQueue.Build(this.crawlingJob.GetRandomHashtags());
            }
            else
            {
                this.hashtagQueue.Build(customTags);
            }
        }

        private IEnumerable<string> GetShortCodes(string hashTag)
        {
            var shortcodes = this.crawlingJob.GetShortcodesFromHashtag(hashTag);
            return shortcodes;
        }

        private IImage Crawl(string shortcode)
        {
            return this.crawlingJob.GetImageDataFromShortcode(shortcode);
        }
    }
}
