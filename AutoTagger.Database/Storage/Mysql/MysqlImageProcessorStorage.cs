namespace AutoTagger.Database.Storage.Mysql
{
    using System.Collections.Generic;
    using System.Linq;

    using global::AutoTagger.Contract;
    using global::AutoTagger.Database.Mysql;

    public class MysqlImageProcessorStorage : MysqlBaseStorage
    {
        public IEnumerable<IImage> GetImagesWithoutMáchineTags(int limit)
        {
            var query = (from p in this.db.Photos
                         where p.Mtags.Count == 0
                         select p).Take(limit);
            foreach (var photo in query)
            {
                var image = photo.ToImage();
                yield return image;
            }
        }

        public void InsertMachineTags(IImage image)
        {
            foreach (var mTagName in image.MachineTags)
            {
                this.db.Mtags.Add(new Mtags { Name = mTagName, PhotoId = image.Id });
            }

            this.Save(() => this.InsertMachineTags(image));
        }
    }
}
