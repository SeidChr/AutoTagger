namespace AutoTagger.Contract
{
    using System;

    public interface ICrawlerContext : IDisposable
    {
        void InsertOrUpdate(ICrawlerImage crawlerImage);
    }
}
