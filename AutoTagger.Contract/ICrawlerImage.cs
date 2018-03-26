using System.Collections.Generic;

namespace AutoTagger.Contract
{
    public interface ICrawlerImage
    {
        string ImageId { get; }

        string ImageUrl { get; }

        IEnumerable<string> HumanoidTags { get; }

        int Likes { get; }

        int Comments { get; }
    }
}