namespace AutoTagger.Contract
{
    public interface IMachineTag
    {
        string Name { get; set; }

        float Score { get; set; }

        string Source { get; set; }
    }
}
