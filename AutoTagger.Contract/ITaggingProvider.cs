namespace AutoTagger.Contract
{
    using System.Collections.Generic;

    public interface ITaggingProvider
    {
        IEnumerable<IMTag> GetTagsForImageBytes(byte[] imageBytes);

        IEnumerable<IMTag> GetTagsForImageUrl(string imageUrl);
    }
}
