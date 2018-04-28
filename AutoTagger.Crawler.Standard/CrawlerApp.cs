namespace AutoTagger.Crawler.Standard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoTagger.Contract;

    public class CrawlerApp
    {
        private readonly ICrawlerStorage db;
        private readonly List<IHumanoidTag> allHTags;
        private readonly ICrawler crawler;

        public CrawlerApp(ICrawlerStorage db, ICrawler crawler)
        {
            this.db                     =  db;
            this.crawler                =  crawler;
            this.allHTags               =  db.GetAllHumanoidTags().ToList();
            this.crawler.OnHashtagFound += this.HashtagFound;
        }

        public event Action<IImage> OnImageSaved;

        public void DoCrawling(int limit, params string[] customTags)
        {
            var images = this.crawler.DoCrawling(limit, customTags);

            foreach (var image in images)
            {
                foreach (var hTagName in image.HumanoidTags)
                {
                    var exists = this.allHTags.FirstOrDefault(htag => htag.Name == hTagName);
                    if (exists != null)
                        continue;
                    var newHTag = new HumanoidTag { Name = hTagName };
                    this.db.InsertOrUpdateHumaniodTag(newHTag);
                    this.allHTags.Add(newHTag);
                }

                this.db.InsertOrUpdate(image);
                this.ImageFound(image);
            }
        }

        private void HashtagFound(IHumanoidTag hTag)
        {
            this.db.InsertOrUpdateHumaniodTag(hTag);
            var exists = this.allHTags.FirstOrDefault(htag => htag.Name == hTag.Name);
            if (exists == null)
                this.allHTags.Add(hTag);
        }

        private void ImageFound(IImage image)
        {
            this.OnImageSaved?.Invoke(image);
        }
    }
}
