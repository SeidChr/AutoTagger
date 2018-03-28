namespace AutoTagger.Database.Standard
{
    using System.Collections.Generic;

    using AutoTagger.Contract;

    public class CrawlerImage : ICrawlerImage, IAutoTaggerImage
    {
        public int Comments { get; set; }

        public IEnumerable<string> HumanoidTags { get; set; }

        public string ImageId { get; set; }

        public string ImageUrl { get; set; }

        public int Likes { get; set; }

        public IEnumerable<string> MachineTags { get; set; }

        public double Quality { get; set; }
    }
}
