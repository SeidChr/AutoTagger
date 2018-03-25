using System.Collections.Generic;

namespace AutoTagger.Contract
{
    public interface IAutoTaggerDatabase
    {
        void IndertOrUpdate(string imageId, IEnumerable<string> machineTags, IEnumerable<string> humanoidTags);

        IEnumerable<string> FindInstagramTags(IEnumerable<string> machineTags);

        void Remove(string imageId);
    }
}
