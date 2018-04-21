using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Contract
{
    public interface IImageProcessorStorage
    {
        IEnumerable<IImage> GetImagesWithoutMachineTags(int limit);
        void InsertMachineTags(IImage image);
    }
}
