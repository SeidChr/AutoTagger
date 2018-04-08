namespace AutoTagger.Contract
{
    using System.Collections.Generic;

    public interface ICrawler
    {
        IEnumerable<IImage> DoCrawling(int limit, params string[] hashTags);
    }
}
