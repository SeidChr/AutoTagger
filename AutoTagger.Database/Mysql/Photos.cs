using System;
using System.Collections.Generic;

namespace AutoTagger.Database.Mysql
{
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
    }
}
