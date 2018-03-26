namespace AutoTagger.Contract
{
    public interface ICrawlerDatabase
    {
        void InsertOrUpdate(ICrawlerImage crawlerImage);
    }
}
