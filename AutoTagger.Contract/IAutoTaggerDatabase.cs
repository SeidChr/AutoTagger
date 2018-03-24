using System.Collections.Generic;

namespace AutoTagger.Contract
{
    public interface IAutoTaggerDatabase
    {
        void IndertOrUpdate(string imageId, IEnumerable<string> maschineTags, IEnumerable<string> humanoidTags);

        IEnumerable<string> FindInstagramTags(IEnumerable<string> maschineTags);
    }
}
