namespace AutoTagger.Contract
{
    using System.Collections.Generic;

    public interface ITaggingProvider
    {
        IEnumerable<string> GetTagsForImageBytes(byte[] imageBytes);

        IEnumerable<string> GetTagsForImageUrl(string imageUrl);
    }
}
