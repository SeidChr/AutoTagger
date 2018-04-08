namespace AutoTagger.Database.Context.Crawler
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    using global::AutoTagger.Contract;
    using global::AutoTagger.Database.Context;
    using global::AutoTagger.Database.Mysql;

    using MySql.Data.MySqlClient;

    public class MysqlCrawlerStorage : MysqlStorage, ICrawlerStorage
    {
        private MySqlCommand command;

        public void InsertOrUpdate(IImage image)
        {
            this.RemoveIfExisting(image);

            var photo = Photos.FromImage(image);
            this.db.Photos.Add(photo);
            this.db.SaveChanges();
            image.ImageId = photo.Id.ToString();
        }

        private void RemoveIfExisting(IImage image)
        {
            var existingPhoto = this.db.Photos.FirstOrDefault(x => x.ImgId == image.ImageId);
            if (existingPhoto != null)
            {
                this.db.Itags.RemoveRange(this.db.Itags.Where(x => x.PhotoId == existingPhoto.Id));
                this.db.Mtags.RemoveRange(this.db.Mtags.Where(x => x.PhotoId == existingPhoto.Id));
                this.db.Photos.Remove(existingPhoto);
                this.db.SaveChanges();
            }
        }
    }
}
