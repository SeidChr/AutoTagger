using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Crawler.Standard
{
    using AutoTagger.Contract;

    public class MTag : IMTag
    {
        public string Name { get; set; }

        public float Score { get; set; }

        public string Source { get; set; }
    }
}
