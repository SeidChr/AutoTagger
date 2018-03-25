using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Contract
{
    public interface ITaggingProvider
    {
        IEnumerable<string> GetTagsForImageUrl(string imageUrl);

        IEnumerable<string> GetTagsForImageBytes(byte[] imageBytes);
    }
}
