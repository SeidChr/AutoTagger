namespace AutoTagger.Contract
{
    using System.Collections.Generic;

    public interface IAutoTaggerStorage
    {
        IEnumerable<string> FindHumanoidTags(IEnumerable<string> machineTags);
    }
}
