namespace AutoTagger.Contract
{
    using System.Collections.Generic;

    public interface IAutoTaggerStorage
    {
        IEnumerable<string> FindHumanoidTags(List<string> machineTags);
        void Log(string source, string data);
    }
}
