namespace AutoTagger.Database.Mysql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoTagger.Contract;
    using AutoTagger.Crawler.Standard;

    public class Photos
    {
        public Photos()
        {
            this.Mtags        = new HashSet<Mtags>();
            this.PhotoItagRel = new HashSet<PhotoItagRel>();
        }

        public int Comments { get; set; }

        public DateTimeOffset Created { get; set; }

        public int Follower { get; set; }

        public int Following { get; set; }

        public int Id { get; set; }

        public IEnumerable<Itags> Itags
        {
            get
            {
                foreach (var photoItagRel in this.PhotoItagRel)
                {
                    yield return photoItagRel.Itag;
                }
            }
        }

        public string LargeUrl { get; set; }

        public int Likes { get; set; }

        public ICollection<Mtags> Mtags { get; set; }

        public ICollection<PhotoItagRel> PhotoItagRel { get; set; }

        public int Posts { get; set; }

        public string Shortcode { get; set; }

        public string ThumbUrl { get; set; }

        public DateTimeOffset? Uploaded { get; set; }

        public string User { get; set; }

        public static Photos FromImage(IImage image)
        {
            var photo = new Photos
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
            var image = new Image
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
                    this.Mtags.Select(tag => new MTag { Name = tag.Name, Score = tag.Score, Source = tag.Source }),
                HumanoidTags = this.Itags.Select(tag => tag.Name)
            };
            return image;
        }
    }
}
