namespace AutoTagger.Database.Storage.Mysql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::AutoTagger.Contract;
    using global::AutoTagger.Database.Mysql;

    public class MysqlImageProcessorStorage : MysqlBaseStorage, IImageProcessorStorage
    {
        private readonly Random random;

        public MysqlImageProcessorStorage()
        {
            this.random = new Random();
        }

        public void DoSave()
        {
            this.Save();
        }

        public IEnumerable<IImage> GetImagesWithoutMachineTags(int limit)
        {
            return this.db
                .Photos
                .Where(p => p.Mtags.Count == 0 && p.Id > this.GetRandomId())
                .Select(p => p.ToImage())
                .Take(limit);
        }

        public IEnumerable<IImage> GetImagesWithoutMachineTags(int idLargerThan, int limit)
        {
            return this.db
                .Photos
                .Where(p => p.Mtags.Count == 0 && p.Id > idLargerThan)
                .Select(p => p.ToImage())
                .Take(limit);
        }

        public void InsertMachineTagsWithoutSaving(IImage image)
        {
            foreach (var machineTag in image.MachineTags)
            {
                this.db.Mtags.Add(
                    new Mtags
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
            return this.db.Photos.OrderByDescending(p => p.Id).FirstOrDefault()?.Id ?? -1;
        }

        private int GetRandomId()
        {
            var largestId = this.GetLargestId();
            return this.random.Next(1, largestId);
        }
    }
}
