using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Database.Standard
{
    using AutoTagger.Contract;

    public class AutoTaggerRepository : BaseRepository, IAutoTaggerRepository
    {
        public AutoTaggerRepository(IAutoTaggerContext context)
        {
            _context = context;
        }

        public IEnumerable<string> FindHumanoidTags(IEnumerable<string> machineTags)
        {
            return (_context as IAutoTaggerContext).FindHumanoidTags(machineTags);
        }

        public void InsertOrUpdate(string imageId, IEnumerable<string> machineTags, IEnumerable<string> humanoidTags)
        {
            (_context as IAutoTaggerContext).InsertOrUpdate(imageId, machineTags, humanoidTags);
        }

        public void Remove(string imageId)
        {
            (_context as IAutoTaggerContext).Remove(imageId);
        }
    }
}
