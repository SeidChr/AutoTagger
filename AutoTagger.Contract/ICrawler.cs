namespace AutoTagger.Contract
{
    public interface ICrawler
    {
        ICrawlerImage GetCrawlerImageForImageId(string imageId);
    }
}
