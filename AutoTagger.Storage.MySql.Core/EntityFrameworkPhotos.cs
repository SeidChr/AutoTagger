namespace AutoTagger.Storage.EntityFramework.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoTagger.Contract;

    public class EntityFrameworkPhotos
    {
        public EntityFrameworkPhotos()
        {
            this.MachineTags = new HashSet<EntityFrameworkMachineTags>();
            this.PhotoItagRel = new HashSet<EntityFrameworkPhotoItagRel>();
        }

        public int Comments { get; set; }

        public DateTimeOffset Created { get; set; }

        public int Follower { get; set; }

        public int Following { get; set; }

        public int Id { get; set; }

        public IEnumerable<EntityFrameworkHumanoidTags> Itags
        {
            get
            {
                foreach (var photoItagRel in this.PhotoItagRel)
                {
                    yield return photoItagRel.HumanoidTag;
                }
            }
        }

        public string LargeUrl { get; set; }

        public int Likes { get; set; }

        public ICollection<EntityFrameworkMachineTags> MachineTags { get; set; }

        public ICollection<EntityFrameworkPhotoItagRel> PhotoItagRel { get; set; }

        public int Posts { get; set; }

        public string Shortcode { get; set; }

        public string ThumbUrl { get; set; }

        public DateTimeOffset? Uploaded { get; set; }

        public string User { get; set; }

        public static EntityFrameworkPhotos FromImage(IImage image)
        {
            var photo = new EntityFrameworkPhotos
            {
                LargeUrl  = image.LargeUrl,
                ThumbUrl  = image.ThumbUrl,
                Shortcode = image.Shortcode,
                Likes     = image.Likes,
                Comments  = image.Comments,
                User      = image.User,
                Follower  = image.Follower,
                Following = image.Following,
                Posts     = image.Posts,
                Uploaded  = image.Uploaded
            };

            return photo;
        }

        public IImage ToImage()
        {
            var image = new EntityFrameworkImage
            {
                Id        = this.Id,
                LargeUrl  = this.LargeUrl,
                ThumbUrl  = this.ThumbUrl,
                Shortcode = this.Shortcode,
                Likes     = this.Likes,
                Comments  = this.Comments,
                User      = this.User,
                Follower  = this.Follower,
                Following = this.Following,
                Posts     = this.Posts,
                MachineTags =
                    this.MachineTags.Select(tag => new EntityFrameworkMachineTag { Name = tag.Name, Score = tag.Score, Source = tag.Source }),
                HumanoidTags = this.Itags.Select(tag => tag.Name)
            };
            return image;
        }
    }
}
