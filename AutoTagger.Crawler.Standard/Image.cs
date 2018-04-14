namespace AutoTagger.Crawler.Standard
{
    using System.Collections.Generic;

    using AutoTagger.Contract;

    using Newtonsoft.Json;

    public class Image : IImage
    {
        public IEnumerable<string> MachineTags { get; set; }

        public IEnumerable<string> HumanoidTags { get; set; }

        public int Id { get; set; }

        public string Shortcode { get; set; }

        public string Url { get; set; }

        public int Likes { get; set; }

        public int Follower { get; set; }

        public int CommentCount { get; set; }

        public string User { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
