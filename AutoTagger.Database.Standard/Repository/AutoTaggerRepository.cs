namespace AutoTagger.Database.Standard
{
    using System.Collections.Generic;

    using AutoTagger.Contract;
    using AutoTagger.Database.Standard.Repository;

    public class AutoTaggerRepository : BaseRepository, IAutoTaggerRepository
    {
        private readonly IAutoTaggerContext context;

        public AutoTaggerRepository(IAutoTaggerContext context)
            : base(context)
        {
            this.context = context;
        }

        public IEnumerable<string> FindHumanoidTags(IEnumerable<string> machineTags)
        {
            return this.context.FindHumanoidTags(machineTags);
        }

        public void InsertOrUpdate(string imageId, IEnumerable<string> machineTags, IEnumerable<string> humanoidTags)
        {
            this.context.InsertOrUpdate(imageId, machineTags, humanoidTags);
        }

        public void Remove(string imageId)
        {
            this.context.Remove(imageId);
        }
    }
}
