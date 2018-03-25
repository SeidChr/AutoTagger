using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AutoTagger.Contract
{
    public interface ICrawlerDatabase
    {
        void InsertOrUpdate(ICrawlerImage crawlerImage);
    }
}
