using System;
using System.Collections.Generic;

namespace AutoTagger.Database.Mysql
{
    using System.Linq;

    using AutoTagger.Contract;
    using AutoTagger.Crawler.Standard;

    public class Photos
    {
        public Photos()
        {
            Mtags = new HashSet<Mtags>();
            PhotoItagRel = new HashSet<PhotoItagRel>();
        }

        public int Id { get; set; }
        public string LargeUrl { get; set; }
        public string ThumbUrl { get; set; }
        public string Shortcode { get; set; }
        public int Likes { get; set; }
        public int Comments { get; set; }
        public string User { get; set; }
        public int Follower { get; set; }
        public int Following { get; set; }
        public int Posts { get; set; }
        public DateTimeOffset? Uploaded { get; set; }
        public DateTimeOffset Created { get; set; }

        public ICollection<Mtags> Mtags { get; set; }
        public ICollection<PhotoItagRel> PhotoItagRel { get; set; }

        public static Photos FromImage(IImage image)
        {
            var photo = new Photos
            {
                LargeUrl       = image.LargeUrl,
                ThumbUrl       = image.ThumbUrl,
                Shortcode = image.Shortcode,
                Likes     = image.Likes,
                Comments  = image.Comments,
                User      = image.User,
                Follower = image.Follower,
                Following  = image.Following,
                Posts  = image.Posts,
                Uploaded  = image.Uploaded
            };

            return photo;
        }

        public IEnumerable<Itags> Itags
        {
            get
            {
                foreach (var photoItagRel in PhotoItagRel)
                {
                    yield return photoItagRel.Itag;
                }
            }
        }

        public IImage ToImage()
        {
            var image = new Image
            {
                Id = this.Id,
                LargeUrl  = this.LargeUrl,
                ThumbUrl  = this.ThumbUrl,
                Shortcode = this.Shortcode,
                Likes     = this.Likes,
                Comments  = this.Comments,
                User      = this.User,
                Follower  = this.Follower,
                Following = this.Following,
                Posts     = this.Posts,
                MachineTags = this.Mtags.Select(tag => new MTag{Name= tag.Name , Score=tag.Score, Source = tag.Source}),
                HumanoidTags = this.Itags.Select(tag => tag.Name)
            };
            return image;
        }
    }
}
