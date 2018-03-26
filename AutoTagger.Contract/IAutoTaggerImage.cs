namespace AutoTagger.Contract
{
    using System.Collections.Generic;

    public interface IAutoTaggerImage
    {
        IEnumerable<string> HumanoidTags { get; }

        string ImageId { get; }

        IEnumerable<string> MachineTags { get; }

        double Quality { get; }
    }
}
