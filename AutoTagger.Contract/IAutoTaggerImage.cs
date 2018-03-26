using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Contract
{
    public interface IAutoTaggerImage
    {
        IEnumerable<string> MachineTags { get; }

        IEnumerable<string> HumanoidTags { get; }

        string ImageId { get; }

        double Quality { get; }
    }
}
