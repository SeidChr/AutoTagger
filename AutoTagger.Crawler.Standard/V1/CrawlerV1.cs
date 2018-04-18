namespace AutoTagger.Crawler.Standard.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoTagger.Contract;
    using AutoTagger.Crawler.Standard.V1;
    using AutoTagger.Crawler.Standard.V1.Crawler;

    public class CrawlerV1 : ICrawler
    {
        private readonly HashtagQueue<IHumanoidTag> hashtagQueue;
        private readonly RandomTagsCrawler randomTagsCrawler;
        private readonly ExploreTagsCrawler exploreTagsPageCrawler;
        private readonly ImageDetailCrawler imageDetailPageCrawler;
        private readonly UserCrawler userCrawler;
        private readonly ICrawlerStorage db;
        private List<IHumanoidTag> allTags;

        public CrawlerV1(ICrawlerStorage db)
        {
            this.db = db;
            this.hashtagQueue = new HashtagQueue<IHumanoidTag>();
            this.randomTagsCrawler      = new RandomTagsCrawler();
            this.exploreTagsPageCrawler = new ExploreTagsCrawler();
            this.imageDetailPageCrawler = new ImageDetailCrawler();
            this.userCrawler            = new UserCrawler();

            List<IHumanoidTag> preexistingHTags = db.GetAllHumanoidTags().ToList();
            this.allTags = preexistingHTags;
            this.hashtagQueue.AddProcessed(preexistingHTags);
        }

        public void DoCrawling(int limit, params string[] customTags)
        {
            this.BuildTags(customTags);
            this.hashtagQueue.SetLimit(limit);
            this.hashtagQueue.OnHashtagFound += OnHashtagFound;
            var images = this.hashtagQueue.Process(
                this.ExploreTagsCrawlerFunc,
                this.ImagePageCrawlerFunc,
                this.UserCrawlerFunc
                );

            foreach (var image in images)
            {
                foreach (var hTagName in image.HumanoidTags)
                {
                    var exists = allTags.FirstOrDefault(htag => htag.Name == hTagName);
                    if (exists != null)
                        continue;
                    var newHTag = new HumanoidTag { Name = hTagName };
                    this.db.InsertOrUpdateHumaniodTag(newHTag);
                    allTags.Add(newHTag);
                }
                this.db.InsertOrUpdate(image);
            }
        }

        private void OnHashtagFound(IHumanoidTag hTag)
        {
            this.db.InsertOrUpdateHumaniodTag(hTag);
            allTags.Add(hTag);
        }

        private void BuildTags(string[] customTags)
        {
            var tags = customTags.Length == 0 ? this.randomTagsCrawler.Parse() : customTags;
            var hTags = new List<IHumanoidTag>();
            foreach (var name in tags)
            {
                hTags.Add(new HumanoidTag { Name = name });
            }
            this.hashtagQueue.Build(hTags);
        }

        private (int, List<string>) ExploreTagsCrawlerFunc(IHumanoidTag tag)
        {
            var url = $"https://www.instagram.com/explore/tags/{tag.Name}/";
            (var amountPosts, var images) = this.exploreTagsPageCrawler.Parse(url);
            var shortcodes = new List<string>();
            if (images != null)
            {
                foreach (var image in images)
                {
                    shortcodes.Add(image.Shortcode);
                }
            }
            return (amountPosts, shortcodes);
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
