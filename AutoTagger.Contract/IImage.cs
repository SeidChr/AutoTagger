namespace AutoTagger.Contract
{
    using System;
    using System.Collections.Generic;

    public interface IImage
    {
        IEnumerable<IMTag> MachineTags { get; set; }
        IEnumerable<string> HumanoidTags { get; set; }
        int Id { get; set; }
        string Shortcode { get; set; }
        string LargeUrl { get; set; }
        string ThumbUrl { get; set; }
        int Likes { get; set; }
        int Comments { get; set; }
        string User { get; set; }
        int Follower { get; set; }
        int Following { get; set; }
        int Posts { get; set; }
        DateTime Uploaded { get; set; }
    }
}
