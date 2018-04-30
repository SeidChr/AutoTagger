using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Test.Core
{
    using AutoTagger.Clarifai.Standard;

    using Xunit;

    public class ImageProcessing
    {
        [Fact]
        public void GCPVision_FromUri()
        {
            var tagger = new GCPVision();
            var uri = "https://www.bilderdepot24.de/item/images/1149747/1000x1000/1149747_1.jpg";
            var result = tagger.GetTagsForImageUrl(uri);
            foreach (var res in result)
            {
                Console.WriteLine(res);
            }
        }
    }
}
