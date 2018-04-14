using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Crawler.Standard
{
    using AutoTagger.Contract;

    public interface ICrawlingJob
    {
        IImage GetImageDataFromShortcode(string shortcode);
        IEnumerable<string> GetRandomHashtags();
        IEnumerable<string> GetShortcodesFromHashtag(string hashTag);
    }
}
