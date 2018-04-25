using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Contract
{
    public interface IHumanoidTag
    {
        string Name { get; set; }
        int Posts { get; set; }
    }
}
