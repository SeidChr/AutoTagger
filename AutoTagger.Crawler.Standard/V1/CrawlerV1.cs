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
        private readonly ImagesOverviewPageCrawler exploreTagsPageCrawler;
        private readonly ImageDetailPageCrawler imageDetailPageCrawler;
        //private readonly UserPageCrawler userPageCrawler;

        public CrawlerV1()
        {
            this.hashtagQueue = new HashtagQueue<string>();

            this.randomTagsCrawler = new RandomTagsCrawler();
            this.exploreTagsPageCrawler = new ImagesOverviewPageCrawler();
            this.imageDetailPageCrawler = new ImageDetailPageCrawler();
            //this.userPageCrawler = new UserPageCrawler();
        }

        public IEnumerable<IImage> DoCrawling(int limit, params string[] customTags)
        {
            this.BuildQueue(customTags);
            this.hashtagQueue.SetLimit(limit);
            return this.hashtagQueue.Process(
                this.exploreTagsCrawlerFunc,
                this.imagePageCrawlerFunc,
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
            var images = this.exploreTagsPageCrawler.Parse(url, ImagesOverviewPageCrawler.PageType.ExploreTags);
            foreach (var image in images)
            {
                yield return image.Shortcode;
            }
        }

        private string imagePageCrawlerFunc(string shortcode)
        {
            var url = $"https://www.instagram.com/p/{shortcode}/?hl=en";
            return this.imageDetailPageCrawler.Parse(url);
        }

        private IEnumerable<IImage> userCrawlerFunc(string userName)
        {
            var url = $"https://www.instagram.com/{userName}/?hl=en";
            var images = this.exploreTagsPageCrawler.Parse(url, ImagesOverviewPageCrawler.PageType.Profile);
            foreach (var image in images)
            {
                image.User = userName;
                yield return image;
            }
        }
    }
}
