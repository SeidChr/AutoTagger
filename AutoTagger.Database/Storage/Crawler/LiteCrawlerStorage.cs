namespace AutoTagger.Database.Storage.Crawler
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
                    ["_id"]  = image.Shortcode,
                    ["url"]  = image.LargeUrl,
                    ["tags"] = new BsonArray(image.HumanoidTags.Select(t => new BsonValue(t)))
                };
            }

            IImage ImageFromBson(BsonValue value)
            {
                var doc = value.AsDocument;
                return new Image
                {
                    Shortcode      = doc["_id"],
                    LargeUrl     = doc["url"],
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
            return this.images.Find(x => x.Shortcode == imageId).Any();
        }

        public IEnumerable<string> CountTags()
        {
            return this.images.FindAll().Select(x => x.Shortcode);
        }

        public IEnumerable<string> GetImageIds()
        {
            return this.images.FindAll().Select(x => x.Shortcode);
        }

        public void InsertOrUpdate(IImage crawlerImage)
        {
            this.images.Upsert(crawlerImage);
        }

        public IEnumerable<IHumanoidTag> GetAllHumanoidTags()
        {
            throw new System.NotImplementedException();
        }

        public void InsertOrUpdateHumaniodTag(IHumanoidTag hTag)
        {
            throw new System.NotImplementedException();
        }
    }
}
