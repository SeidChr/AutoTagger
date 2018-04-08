namespace AutoTagger.Database.Context.Crawler
{
    using System.Collections.Generic;
    using System.Linq;
    using global::AutoTagger.Contract;
    using global::AutoTagger.Crawler.Standard;

    using LiteDB;

    public class LiteCrawlerStorage : ICrawlerStorage
    {
        private readonly LiteCollection<IImage> images;

        public LiteCrawlerStorage(string fileName)
        {
            BsonDocument BsonFromImage(IImage image)
            {
                return new BsonDocument
                {
                    ["_id"]  = image.ImageId,
                    ["url"]  = image.ImageUrl,
                    ["tags"] = new BsonArray(image.HumanoidTags.Select(t => new BsonValue(t)))
                };
            }

            IImage ImageFromBson(BsonValue value)
            {
                var doc = value.AsDocument;
                return new Image
                {
                    ImageId      = doc["_id"],
                    ImageUrl     = doc["url"],
                    HumanoidTags = doc["tags"].AsArray.Select(t => t.AsString)
                };
            }

            var mapper = new BsonMapper();
            mapper.RegisterType(BsonFromImage, ImageFromBson);

            var database = new LiteDatabase(fileName, mapper);
            this.images   = database.GetCollection<IImage>("images");
        }

        public bool Contains(string imageId)
        {
            return this.images.Find(x => x.ImageId == imageId).Any();
        }

        public IEnumerable<string> CountTags()
        {
            return this.images.FindAll().Select(x => x.ImageId);
        }

        public IEnumerable<string> GetImageIds()
        {
            return this.images.FindAll().Select(x => x.ImageId);
        }

        public void InsertOrUpdate(IImage crawlerImage)
        {
            this.images.Upsert(crawlerImage);
        }
    }
}
