using System;
using System.Collections.Generic;

namespace AutoTagger.Database.Mysql
{
    public partial class Mtags
    {
        public int Id { get; set; }
        public int PhotoId { get; set; }
        public string Name { get; set; }
        public float Score { get; set; }
        public string Source { get; set; }

        public Photos Photo { get; set; }
    }
}
