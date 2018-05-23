namespace AutoTagger.Storage.EntityFramework.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoTagger.Contract;

    public class EntityFrameworkImageProcessorStorage : EntityFrameworkBaseStorage, IImageProcessorStorage
    {
        private readonly Random random;

        public EntityFrameworkImageProcessorStorage()
        {
            this.random = new Random();
        }

        public void DoSave()
        {
            this.Save();
        }

        public IEnumerable<IImage> GetImagesWithoutMachineTags(int limit)
        {
            return this.Db
                .Photos
                .Where(p => p.MachineTags.Count == 0 && p.Id > this.GetRandomId())
                .Select(p => p.ToImage())
                .Take(limit).ToList();
        }

        public IEnumerable<IImage> GetImagesWithoutMachineTags(int idLargerThan, int limit)
        {
            return this.Db
                .Photos
                .Where(p => p.MachineTags.Count == 0 && p.Id > idLargerThan)
                .Select(p => p.ToImage())
                .Take(limit);
        }

        public void InsertMachineTagsWithoutSaving(IImage image)
        {
            foreach (var machineTag in image.MachineTags)
            {
                this.Db.MachineTags.Add(
                    new EntityFrameworkMachineTags
                    {
                        Name    = machineTag.Name,
                        Score   = machineTag.Score,
                        Source  = machineTag.Source,
                        PhotoId = image.Id
                    });
            }
        }

        private int GetLargestId()
        {
            return this.Db.Photos.OrderByDescending(p => p.Id).FirstOrDefault()?.Id ?? -1;
        }

        private int GetRandomId()
        {
            var largestId = this.GetLargestId();
            return this.random.Next(1, largestId);
        }
    }
}
