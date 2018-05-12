namespace AutoTagger.Contract
{
    public interface IMTag
    {
        string Name { get; set; }

        float Score { get; set; }

        string Source { get; set; }
    }
}
