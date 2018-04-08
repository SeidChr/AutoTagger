using System;
using System.Collections.Generic;

namespace AutoTagger.Database.Mysql
{
    using AutoTagger.Contract;

    public partial class Photos
    {
        public Photos()
        {
            Itags = new HashSet<Itags>();
            Mtags = new HashSet<Mtags>();
        }

        public static Photos FromImage(IImage image)
        {
            var photo = new Photos
            {
                Img      = image.ImageUrl,
                Likes    = image.Likes,
                Comments = image.Comments,
                Follower = image.Follower
            };
            foreach (var iTag in image.HumanoidTags)
            {
                photo.Itags.Add(new Itags {Value = iTag });
            }
            foreach (var mTag in image.MachineTags)
            {
                photo.Mtags.Add(new Mtags { Value = mTag });
            }
            return photo;
        }

        public int Id { get; set; }
        public string Img { get; set; }
        public int Likes { get; set; }
        public int Comments { get; set; }
        public int Follower { get; set; }

        public ICollection<Itags> Itags { get; set; }
        public ICollection<Mtags> Mtags { get; set; }
    }
}
