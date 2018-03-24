using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Database.Standard
{
    public interface ITaggedDatabase
    {
        void Add(string image, string[] automaticTags, string[] instagramTags);

        string[] FindInstagramTags(string[] automaticTags);
    }
}
