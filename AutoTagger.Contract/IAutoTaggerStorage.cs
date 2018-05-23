namespace AutoTagger.Contract
{
    using System.Collections.Generic;

    public interface IAutoTaggerStorage
    {
        (string debug, IEnumerable<string> htags) FindHumanoidTags(List<IMachineTag> machineTags);

        void Log(string source, string data);
    }
}
