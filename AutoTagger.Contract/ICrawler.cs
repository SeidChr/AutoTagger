namespace AutoTagger.Contract
{
    using System.Collections.Generic;

    public interface ICrawler
    {
        IEnumerable<IImage> DoCrawling(int amount, params string[] hashTags);
        IImage GetCrawlerImageForImageId(string imageId);
    }
}
