namespace AutoTagger.Crawler.Standard
{
    using AutoTagger.Contract;

    public class MachineTag : IMachineTag
    {
        public string Name { get; set; }

        public float Score { get; set; }

        public string Source { get; set; }
    }
}
