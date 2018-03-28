namespace AutoTagger.Contract
{
    public interface ICrawlerContext : IContext
    {
        void InsertOrUpdate(ICrawlerImage crawlerImage);
    }
}
