using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Contract
{
    public interface ICrawlerRepository
    {
        void InsertOrUpdate(ICrawlerImage crawlerImage);
    }
}
