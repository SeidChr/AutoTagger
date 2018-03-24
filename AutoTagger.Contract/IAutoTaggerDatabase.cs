namespace AutoTagger.Contract
{
    public interface ITaggedDatabase
    {
        void Add(string image, string[] automaticTags, string[] instagramTags);

        string[] FindInstagramTags(string[] automaticTags);
    }
}
