namespace AutoTagger.Contract
{
    using System.Collections.Generic;

    public interface IAutoTaggerImage : ICrawlerImage
    {
        IEnumerable<string> MachineTags { get; }

        double Quality { get; }
    }
}
