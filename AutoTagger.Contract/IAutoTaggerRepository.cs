using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Contract
{
    public interface IAutoTaggerRepository
    {
        IEnumerable<string> FindHumanoidTags(IEnumerable<string> machineTags);
        void Remove(string imageId);
        void InsertOrUpdate(string imageId, IEnumerable<string> machineTags, IEnumerable<string> humanoidTags);
    }
}
