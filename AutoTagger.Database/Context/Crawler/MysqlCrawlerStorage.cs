namespace AutoTagger.Database.Context.Crawler
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using global::AutoTagger.Contract;
    using global::AutoTagger.Database.Context;
    using global::AutoTagger.Database.Mysql;
    using MySql.Data.MySqlClient;

    public class MysqlCrawlerStorage : MysqlStorage, ICrawlerStorage
    {
        private List<Itags> allITags;

        public void InsertOrUpdate(IImage image)
        {
            this.RemoveIfExisting(image);

            var photo = Photos.FromImage(image);
            if (image.HumanoidTags != null)
            {
                foreach (var iTagName in image.HumanoidTags)
                {
                    var itag = this.allITags.SingleOrDefault(x => x.Name == iTagName);
                    if(itag == null)
                    {
                        itag = new Itags { Name = iTagName };
                    }
                    var rel = new PhotoItagRel { Itag = itag, Photo = photo };
                    photo.PhotoItagRel.Add(rel);
                }
            }

            this.db.Photos.Add(photo);
            this.Save(image);
            image.Shortcode = photo.Id.ToString();
        }

        private void Save(IImage image)
        {
            try
            {
                this.db.SaveChanges();
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Timeout"))
                {
                    this.Reconnect();
                    this.InsertOrUpdate(image);
                }
            }
        }

        private void RemoveIfExisting(IImage image)
        {
            var existingPhoto = this.db.Photos.FirstOrDefault(x => x.Shortcode == image.Shortcode);
            if (existingPhoto != null)
            {
                this.db.PhotoItagRel.RemoveRange(this.db.PhotoItagRel.Where(x => x.PhotoId == existingPhoto.Id));
                this.db.Mtags.RemoveRange(this.db.Mtags.Where(x => x.PhotoId == existingPhoto.Id));
                this.db.Photos.Remove(existingPhoto);
                this.db.SaveChanges();
            }
        }

        public List<Itags> GetAllITags()
        {
            return this.allITags = this.db.Itags.ToList();
        }
    }
}
