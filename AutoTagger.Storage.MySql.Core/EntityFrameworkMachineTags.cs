namespace AutoTagger.Storage.EntityFramework.Core
{
    public class EntityFrameworkMachineTags
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public EntityFrameworkPhotos EntityFrameworkPhoto { get; set; }

        public int PhotoId { get; set; }

        public float Score { get; set; }

        public string Source { get; set; }
    }
}
