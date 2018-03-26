namespace AutoTagger.Contract
{
    using System.Collections.Generic;

    public interface ICrawlerImage
    {
        IEnumerable<string> HumanoidTags { get; }

        string ImageId { get; }

        string ImageUrl { get; }

        IEnumerable<string> HumanoidTags { get; }

        int Likes { get; }

        int Comments { get; }
    }
}
