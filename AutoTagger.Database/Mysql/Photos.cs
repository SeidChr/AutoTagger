using System;
using System.Collections.Generic;

namespace AutoTagger.Database.Mysql
{
    using AutoTagger.Contract;

    public partial class Photos
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

            if (image.MachineTags != null)
            {
                foreach (var mtagName in image.MachineTags)
                {
                    photo.Mtags.Add(new Mtags { Name = mtagName });
                }
            }

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
    }
}
