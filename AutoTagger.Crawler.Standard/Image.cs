namespace AutoTagger.Crawler.Standard
{
    using System;
    using System.Collections.Generic;

    using AutoTagger.Contract;

    using Newtonsoft.Json;

    public class Image : IImage
    {
        public int Comments { get; set; }

        public int Follower { get; set; }

        public int Following { get; set; }

        public IEnumerable<string> HumanoidTags { get; set; }

        public int Id { get; set; }

        public string LargeUrl { get; set; }

        public int Likes { get; set; }

        public IEnumerable<IMTag> MachineTags { get; set; }

        public int Posts { get; set; }

        public string Shortcode { get; set; }

        public string ThumbUrl { get; set; }

        public DateTime Uploaded { get; set; }

        public string User { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
