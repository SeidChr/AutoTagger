namespace AutoTagger.Database.Mysql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Itags
    {
        public Itags()
        {
            this.PhotoItagRel = new HashSet<PhotoItagRel>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<PhotoItagRel> PhotoItagRel { get; set; }

        public IEnumerable<Photos> Photos
        {
            get
            {
                return this.PhotoItagRel.Select(photoItagRel => photoItagRel.Photo);
            }
        }

        public int Posts { get; set; }

        public DateTimeOffset Updated { get; set; }
    }
}
