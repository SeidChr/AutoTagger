using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace AutoTagger.TaggingProvider
{
    public class TagMobule : NancyModule
    {
        public TagMobule()
        {
            Get["/find"] = parameters => "Hello World! :)";
        }
    }
}
