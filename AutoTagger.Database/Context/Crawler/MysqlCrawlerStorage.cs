namespace AutoTagger.Database.Context.Crawler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using global::AutoTagger.Contract;
    using global::AutoTagger.Crawler.Standard;
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
                        throw new InvalidOperationException("ITag must exists in DB");
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

        public IEnumerable<IHumanoidTag> GetAllHumanoidTags()
        {
            this.allITags = this.db.Itags.ToList();
            var hTags = new List<HumanoidTag>();
            foreach (var iTag in this.allITags)
            {
                hTags.Add(new HumanoidTag{Name = iTag.Name, Posts = iTag.Posts});
            }
            return hTags;
        }

        public void InsertOrUpdateITag(IHumanoidTag iTag)
        {
            iTag.Name = iTag.Name.ToLower();

            var existingITag = this.db.Itags.FirstOrDefault(x => x.Name == iTag.Name);
            if (existingITag != null)
            {
                if (existingITag.Posts == iTag.Posts)
                    return;

                existingITag.Posts = iTag.Posts;
                this.db.Itags.Update(existingITag);
                this.Save(() => this.InsertOrUpdateITag(iTag));
            }
            else
            {
                var itag = new Itags { Name = iTag.Name, Posts = iTag.Posts };
                this.db.Itags.Add(itag);
                if (this.Save(() => this.InsertOrUpdateITag(iTag)))
                {
                    this.allITags.Add(itag);
                }
            }
        }
    }
}
