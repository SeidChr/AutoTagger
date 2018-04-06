namespace AutoTagger.Crawler.Standard
{
    using System.Collections.Generic;

    using AutoTagger.Contract;

    public class CrawlerImage : ICrawlerImage
    {
        public int Comments { get; set; }

        public IEnumerable<string> HumanoidTags { get; set; }

        public string ImageId { get; set; }

        public string ImageUrl { get; set; }

        public int Likes { get; set; }

        public int AuthorPopularity { get; set; }
    }
}
