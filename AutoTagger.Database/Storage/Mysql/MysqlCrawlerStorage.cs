namespace AutoTagger.Database.Storage.Mysql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::AutoTagger.Contract;
    using global::AutoTagger.Crawler.Standard;
    using global::AutoTagger.Database.Storage;
    using global::AutoTagger.Database.Mysql;

    public class MysqlCrawlerStorage : MysqlBaseStorage, ICrawlerStorage
    {
        private List<Itags> allITags;

        public void InsertOrUpdate(IImage image)
        {
            var photo = Photos.FromImage(image);

            if (this.TryUpdate(photo))
                return;

            this.Insert(image, photo);
        }

        private bool TryUpdate(Photos photo)
        {
            var existingPhoto = this.db.Photos.FirstOrDefault(x => x.Shortcode == photo.Shortcode);
            if (existingPhoto == null)
                return false;

            existingPhoto.Likes = photo.Likes;
            existingPhoto.Comments = photo.Comments;
            existingPhoto.Following = photo.Following;
            existingPhoto.Posts = photo.Posts;
            this.Save();
            return true;
        }

        private void Insert(IImage image, Photos photo)
        {
            if (image.HumanoidTags == null)
                return;

            foreach (var iTagName in image.HumanoidTags)
            {
                var itag = this.allITags.SingleOrDefault(x => x.Name == iTagName);
                if (itag == null)
                {
                    throw new InvalidOperationException("ITag must exists in DB");
                }

                var rel = new PhotoItagRel { Itag = itag, Photo = photo };
                photo.PhotoItagRel.Add(rel);
            }

            this.db.Photos.Add(photo);
            this.Save();
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

        public void InsertOrUpdateHumaniodTag(IHumanoidTag hTag)
        {
            hTag.Name = hTag.Name.ToLower();

            var existingITag = this.allITags.FirstOrDefault(x => x.Name == hTag.Name);
            if (existingITag != null)
            {
                if (existingITag.Posts == hTag.Posts)
                    return;

                existingITag.Posts = hTag.Posts;
                this.db.Itags.Update(existingITag);
                this.Save();
            }
            else
            {
                var itag = new Itags { Name = hTag.Name, Posts = hTag.Posts };
                this.db.Itags.Add(itag);
                this.Save();
                this.allITags.Add(itag);
            }
        }

    }
}
