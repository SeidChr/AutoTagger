using System;
using System.Collections.Generic;
using System.Text;
using AutoTagger.Contract;

namespace AutoTagger.Database.Standard
{
    public class LiteCrawlerImage : ICrawlerImage, IAutoTaggerImage
    {
        public string ImageId { get; set; }

        public double Quality { get; set; }

        public string ImageUrl { get; set; }

        public IEnumerable<string> MachineTags { get; set; }

        public IEnumerable<string> HumanoidTags { get; set; }
    }
}
