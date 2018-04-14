namespace AutoTagger.Contract
{
    using System.Collections.Generic;

    public interface IImage
    {
        IEnumerable<string> MachineTags { get; set; }

        IEnumerable<string> HumanoidTags { get; set; }

        int Id { get; set; }

        string ImageId { get; set; } // Shortcode

        string ImageUrl { get; set; }

        string InstaUrl { get; set; }

        int Likes { get; set; }

        int Follower { get; set; }

        int CommentCount { get; set; }

        string User { get; set; }
    }
}
