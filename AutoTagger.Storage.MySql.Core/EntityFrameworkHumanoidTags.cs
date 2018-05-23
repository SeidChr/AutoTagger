namespace AutoTagger.Storage.EntityFramework.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EntityFrameworkHumanoidTags
    {
        public EntityFrameworkHumanoidTags()
        {
            this.PhotoItagRel = new HashSet<EntityFrameworkPhotoItagRel>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<EntityFrameworkPhotoItagRel> PhotoItagRel { get; set; }

        public IEnumerable<EntityFrameworkPhotos> Photos
        {
            get
            {
                return this.PhotoItagRel.Select(photoItagRel => photoItagRel.EntityFrameworkPhoto);
            }
        }

        public int Posts { get; set; }

        public DateTimeOffset Updated { get; set; }
    }
}
