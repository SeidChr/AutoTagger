namespace AutoTagger.Contract
{
    using System;

    public interface ICrawlerStorage : IDisposable
    {
        void InsertOrUpdate(ICrawlerImage crawlerImage);
    }
}
