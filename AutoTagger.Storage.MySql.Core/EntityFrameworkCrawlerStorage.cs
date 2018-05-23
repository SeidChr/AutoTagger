namespace AutoTagger.Storage.EntityFramework.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoTagger.Contract;
    using AutoTagger.Crawler.Standard;

    public class EntityFrameworkCrawlerStorage : EntityFrameworkBaseStorage, ICrawlerStorage
    {
        private List<EntityFrameworkHumanoidTags> allHumanoidTags;

        public IEnumerable<IHumanoidTag> GetAllHumanoidTags()
        {
            this.allHumanoidTags = this.Db.HumanoidTags.ToList();
            var humanoidTags = new List<HumanoidTag>();
            foreach (var humanoidTag in this.allHumanoidTags)
            {
                humanoidTags.Add(new HumanoidTag { Name = humanoidTag.Name, Posts = humanoidTag.Posts });
            }

            return humanoidTags;
        }

        public void InsertOrUpdate(IImage image)
        {
            var photo = EntityFrameworkPhotos.FromImage(image);

            if (this.TryUpdate(photo))
            {
                return;
            }

            this.Insert(image, photo);
        }

        public void InsertOrUpdateHumaniodTag(IHumanoidTag humanoidTag)
        {
            humanoidTag.Name = humanoidTag.Name.ToLower();

            var existingITag = this.allHumanoidTags.FirstOrDefault(x => x.Name == humanoidTag.Name);
            if (existingITag != null)
            {
                if (existingITag.Posts == humanoidTag.Posts)
                {
                    return;
                }

                existingITag.Posts = humanoidTag.Posts;
                this.Db.HumanoidTags.Update(existingITag);
                this.Save();
            }
            else
            {
                var itag = new EntityFrameworkHumanoidTags { Name = humanoidTag.Name, Posts = humanoidTag.Posts };
                this.Db.HumanoidTags.Add(itag);
                this.Save();
                this.allHumanoidTags.Add(itag);
            }
        }

        private void Insert(IImage image, EntityFrameworkPhotos entityFrameworkPhoto)
        {
            if (image.HumanoidTags == null)
            {
                return;
            }

            foreach (var humanoidTagName in image.HumanoidTags)
            {
                var itag = this.allHumanoidTags.SingleOrDefault(x => x.Name == humanoidTagName);
                if (itag == null)
                {
                    throw new InvalidOperationException("ITag must exists in DB");
                }

                var rel = new EntityFrameworkPhotoItagRel
                {
                    HumanoidTag          = itag,
                    EntityFrameworkPhoto = entityFrameworkPhoto
                };
                entityFrameworkPhoto.PhotoItagRel.Add(rel);
            }

            this.Db.Photos.Add(entityFrameworkPhoto);
            this.Save();
        }

        private bool TryUpdate(EntityFrameworkPhotos entityFrameworkPhoto)
        {
            var existingPhoto = this.Db.Photos.FirstOrDefault(x => x.Shortcode == entityFrameworkPhoto.Shortcode);
            if (existingPhoto == null)
            {
                return false;
            }

            existingPhoto.Likes     = entityFrameworkPhoto.Likes;
            existingPhoto.Comments  = entityFrameworkPhoto.Comments;
            existingPhoto.Following = entityFrameworkPhoto.Following;
            existingPhoto.Posts     = entityFrameworkPhoto.Posts;

            this.Save();

            return true;
        }
    }
}
