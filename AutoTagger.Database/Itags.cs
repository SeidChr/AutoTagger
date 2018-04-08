using System;
using System.Collections.Generic;

namespace AutoTagger.Database
{
    public partial class Itags
    {
        public int Id { get; set; }
        public int PhotoId { get; set; }
        public string Value { get; set; }

        public Photos Photo { get; set; }
    }
}
