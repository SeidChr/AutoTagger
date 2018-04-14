using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Crawler.Standard
{
    using AutoTagger.Contract;

    public interface ICrawler
    {
        IEnumerable<string> Parse();
    }
}
