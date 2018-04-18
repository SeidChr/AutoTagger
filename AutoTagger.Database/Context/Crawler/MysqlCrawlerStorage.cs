namespace AutoTagger.Database.Context.Crawler
{
    using System;
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
            if(this.Save(() => this.InsertOrUpdate(image)))
            {
                image.Shortcode = photo.Id.ToString();
            }
        }

        private bool Save(Action reconnectFunc)
        {
            try
            {
                this.db.SaveChanges();
                return true;
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Timeout"))
                {
                    this.Reconnect();
                    reconnectFunc();
                }

                return false;
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

        public void InsertOrUpdateITag(string name, int posts)
        {
            name = name.ToLower();

            var existingITag = this.db.Itags.FirstOrDefault(x => x.Name == name);
            if (existingITag != null)
            {
                if (existingITag.Posts == posts)
                    return;

                existingITag.Posts = posts;
                this.db.Itags.Update(existingITag);
                this.Save(() => this.InsertOrUpdateITag(name, posts));
            }
            else
            {
                var itag = new Itags { Name = name, Posts = posts };
                this.db.Itags.Add(itag);
                if (this.Save(() => this.InsertOrUpdateITag(name, posts)))
                {
                    this.allITags.Add(itag);
                }
            }
        }
    }
}
