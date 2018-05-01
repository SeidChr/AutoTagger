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

        public IEnumerable<IImage> GetImagesWithoutMachineTags(int limit)
        {
            var query = (from p in this.db.Photos
                         where p.Mtags.Count == 0
                         && p.Id > this.GetRandomId()
                         select p).Take(limit);
            return query.ToList().Select(x => x.ToImage());
        }

        public IEnumerable<IImage> GetImagesWithoutMachineTags(int idLargerThan, int limit)
        {
            var query = (from p in this.db.Photos
                         where p.Mtags.Count == 0
                            && p.Id > idLargerThan
                         select p).Take(limit);
            return query.ToList().Select(x => x.ToImage());
        }

        private int GetRandomId()
        {
            var largestId = this.GetLargestId();
            return this.random.Next(1, largestId);
        }

        private int GetLargestId()
        {
            return this.db.Photos.OrderByDescending(p => p.Id).FirstOrDefault().Id;
        }

        public void InsertMachineTagsWithoutSaving(IImage image)
        {
            foreach (var mTag in image.MachineTags)
            {
                this.db.Mtags.Add(new Mtags {
                    Name = mTag.Name,
                    Score = mTag.Score,
                    Source = mTag.Source,
                    PhotoId = image.Id

                });
            }
        }

        public void DoSave()
        {
            this.Save();
        }
    }
}
