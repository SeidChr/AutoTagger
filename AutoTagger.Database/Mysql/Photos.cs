using System;
using System.Collections.Generic;

namespace AutoTagger.Database.Mysql
{
    public partial class Photos
    {
        public Photos()
        {
            Itags = new HashSet<Itags>();
            Mtags = new HashSet<Mtags>();
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
