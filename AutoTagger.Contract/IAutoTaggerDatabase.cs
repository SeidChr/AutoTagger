namespace AutoTagger.Contract
{
    using System.Collections.Generic;

    public interface IAutoTaggerDatabase
    {
        IEnumerable<string> FindHumanoidTags(IEnumerable<string> machineTags);

        void InsertOrUpdate(string imageId, IEnumerable<string> machineTags, IEnumerable<string> humanoidTags);

        void Remove(string imageId);
    }
}
