namespace AutoTagger.Crawler.Standard
{
    using System.Collections.Generic;

    using AutoTagger.Contract;

    public class Image : IImage
    {
        public IEnumerable<string> MachineTags { get; set; }

        public IEnumerable<string> HumanoidTags { get; set; }

        public int Id { get; set; }

        public string ImageId { get; set; }

        public string ImageUrl { get; set; }

        public string InstaUrl { get; set; }

        public int Likes { get; set; }

        public int Follower { get; set; }

        public int Comments { get; set; }
    }
}
