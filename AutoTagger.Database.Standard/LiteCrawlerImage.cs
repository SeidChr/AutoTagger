using System;
using System.Collections.Generic;
using System.Text;
using AutoTagger.Contract;

namespace AutoTagger.Database.Standard
{
    public class LiteCrawlerImage : ICrawlerImage
    {
        public LiteCrawlerImage(string imageId, string imageUrl, IEnumerable<string> humanoidTags)
        {
            ImageId = imageId;
            ImageUrl = imageUrl;
            HumanoidTags = humanoidTags;
        }

        public string ImageId { get; }
        public string ImageUrl { get; }
        public IEnumerable<string> HumanoidTags { get; }
    }
}
