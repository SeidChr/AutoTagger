using System;
using System.Collections.Generic;

namespace AutoTagger.Database.Mysql
{
    public partial class Itags
    {
        public Itags()
        {
            PhotoItagRel = new HashSet<PhotoItagRel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Posts { get; set; }
        public DateTimeOffset Updated { get; set; }

        public ICollection<PhotoItagRel> PhotoItagRel { get; set; }

        public IEnumerable<Photos> Photos
        {
            get
            {
                foreach (var photoItagRel in PhotoItagRel)
                {
                    yield return photoItagRel.Photo;
                }
            }
        }
    }
}
