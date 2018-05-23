namespace AutoTagger.Storage.EntityFramework.Core
{
    public class EntityFrameworkPhotoItagRel
    {
        public int Id { get; set; }

        public EntityFrameworkHumanoidTags HumanoidTag { get; set; }

        public int ItagId { get; set; }

        public EntityFrameworkPhotos EntityFrameworkPhoto { get; set; }

        public int PhotoId { get; set; }
    }
}
