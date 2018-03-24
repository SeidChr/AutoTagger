namespace AutoTagger.Contract
{
    public interface IAutoTaggerDatabase
    {
        void Add(string image, string[] automaticTags, string[] instagramTags);

        string[] FindInstagramTags(string[] automaticTags);
    }
}
