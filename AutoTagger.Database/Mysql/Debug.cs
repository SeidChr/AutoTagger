using System;
using System.Collections.Generic;

namespace AutoTagger.Database.Mysql
{
    public partial class Debug
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public string Data { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
