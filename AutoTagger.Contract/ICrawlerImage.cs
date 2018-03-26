namespace AutoTagger.Contract
{
    using System.Collections.Generic;

    public interface ICrawlerImage
    {
        int Comments { get; }

        IEnumerable<string> HumanoidTags { get; }

        string ImageId { get; }

        string ImageUrl { get; }

        int Likes { get; }
    }
}
