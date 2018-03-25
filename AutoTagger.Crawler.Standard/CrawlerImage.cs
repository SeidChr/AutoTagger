using System.Collections.Generic;
using AutoTagger.Contract;

namespace AutoTagger.Crawler.Standard
{
    public class CrawlerImage : ICrawlerImage
    {
        public string ImageId { get; set; }

        public string ImageUrl { get; set; }

        public IEnumerable<string> HumanoidTags { get; set; }
    }
}