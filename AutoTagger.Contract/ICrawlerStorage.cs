namespace AutoTagger.Contract
{
    using System;

    public interface ICrawlerStorage
    {
        void InsertOrUpdate(IImage image);
    }
}
