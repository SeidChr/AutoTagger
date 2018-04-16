using System;
using System.Collections.Generic;

namespace AutoTagger.Database.Mysql
{
    public partial class PhotoItagRel
    {
        public int Id { get; set; }
        public int PhotoId { get; set; }
        public int ItagId { get; set; }

        public Itags Itag { get; set; }
        public Photos Photo { get; set; }
    }
}
