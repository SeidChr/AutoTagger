using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Crawler.Standard
{
    using AutoTagger.Contract;

    public class HumanoidTag : IHumanoidTag
    {
        public string Name { get; set; }
        public int Posts { get; set; }
    }
}
