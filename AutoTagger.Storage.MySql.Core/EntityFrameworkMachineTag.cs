namespace AutoTagger.Storage.EntityFramework.Core
{
    using AutoTagger.Contract;

    public class EntityFrameworkMachineTag : IMachineTag
    {
        public string Name { get; set; }

        public float Score { get; set; }

        public string Source { get; set; }
    }
}
