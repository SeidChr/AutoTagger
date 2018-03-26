using System.Collections.Generic;

namespace AutoTagger.Contract
{
    public interface IAutoTaggerDatabase
    {
        void InsertOrUpdate(string imageId, IEnumerable<string> machineTags, IEnumerable<string> humanoidTags);

        IEnumerable<string> FindHumanoidTags(IEnumerable<string> machineTags);

        void Remove(string imageId);
    }
}
