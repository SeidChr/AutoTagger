namespace AutoTagger.Crawler.Standard.V1
{
    using System.Collections.Generic;
    using AutoTagger.Contract;
    using AutoTagger.Crawler.Standard.V1;
    using AutoTagger.Crawler.Standard.V1.Crawler;

    public class CrawlerV1 : ICrawler
    {
        private readonly HashtagQueue<string> hashtagQueue;

        private readonly RandomTagsCrawler randomTagsCrawler;
        private readonly ExploreTagsPageCrawler exploreTagsCrawler;
        private readonly ImagePageCrawler imagePageCrawler;
        //private readonly UserPageCrawler userPageCrawler;

        public CrawlerV1()
        {
            this.hashtagQueue = new HashtagQueue<string>();

            this.randomTagsCrawler = new RandomTagsCrawler();
            this.exploreTagsCrawler = new ExploreTagsPageCrawler();
            this.imagePageCrawler = new ImagePageCrawler();
            //this.userPageCrawler = new UserPageCrawler();
        }

        public IEnumerable<IImage> DoCrawling(int limit, params string[] customTags)
        {
            this.BuildQueue(customTags);
            this.hashtagQueue.SetLimit(limit);
            return this.hashtagQueue.Process(
                this.exploreTagsCrawlerFunc,
                this.imagePageCrawler.Parse,
                this.userCrawlerFunc
                );
        }

        private void BuildQueue(string[] customTags)
        {
            if (customTags.Length == 0)
            {
                this.hashtagQueue.Build(this.randomTagsCrawler.Parse());
            }
            else
            {
                this.hashtagQueue.Build(customTags);
            }
        }

        private IEnumerable<string> exploreTagsCrawlerFunc(string hashTag)
        {
            var url = $"https://www.instagram.com/explore/tags/{hashTag}/";
            var images = this.exploreTagsCrawler.Parse(url);
            foreach (var image in images)
            {
                yield return image.ImageId;
            }
        }

        private IEnumerable<IImage> userCrawlerFunc(string userName)
        {
            var url = $"https://www.instagram.com/{userName}/?hl=en";
            return this.exploreTagsCrawler.Parse(url);
        }
    }
}
