namespace AutoTagger.Database.Standard
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoTagger.Contract;

    using LiteDB;

    public class LiteCrawlerStorage : ICrawlerStorage
    {
        private readonly LiteCollection<ICrawlerImage> images;

        public LiteCrawlerStorage(string fileName)
        {
            BsonDocument BsonFromImage(ICrawlerImage image)
            {
                return new BsonDocument
                {
                    ["_id"]  = image.ImageId,
                    ["url"]  = image.ImageUrl,
                    ["tags"] = new BsonArray(image.HumanoidTags.Select(t => new BsonValue(t)))
                };
            }

            ICrawlerImage ImageFromBson(BsonValue value)
            {
                var doc = value.AsDocument;
                return new AutoTaggerImage
                {
                    ImageId      = doc["_id"],
                    ImageUrl     = doc["url"],
                    HumanoidTags = doc["tags"].AsArray.Select(t => t.AsString)
                };
            }

            var mapper = new BsonMapper();
            mapper.RegisterType(BsonFromImage, ImageFromBson);

            var database = new LiteDatabase(fileName, mapper);
            this.images   = database.GetCollection<ICrawlerImage>("images");
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

        public void InsertOrUpdate(ICrawlerImage crawlerImage)
        {
            this.images.Upsert(crawlerImage);
        }
    }
}
