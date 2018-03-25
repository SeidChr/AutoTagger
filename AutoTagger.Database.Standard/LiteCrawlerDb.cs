using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTagger.Contract;
using LiteDB;

namespace AutoTagger.Database.Standard
{
    public class LiteCrawlerDb : ICrawlerDatabase
    {
        private readonly LiteDatabase database;

        public LiteCrawlerDb(string fileName)
        {
            BsonDocument BsonFromImage(ICrawlerImage image) => new BsonDocument
            {
                ["_id"] = image.ImageId,
                ["url"] = image.ImageUrl,
                ["tags"] = new BsonArray(image.HumanoidTags.Select(t => new BsonValue(t)))
            };

            ICrawlerImage ImageFromBson(BsonValue value)
            {
                var doc = value.AsDocument;
                return new LiteCrawlerImage(doc["_id"], doc["url"], doc["tags"].AsArray.Select(t => t.AsString));
            }

            var mapper = new BsonMapper();
            mapper.RegisterType<ICrawlerImage>(BsonFromImage, ImageFromBson);
            
            //.Global.RegisterType<Uri>
            //(
            //    serialize: (uri) => uri.AbsoluteUri,
            //    deserialize: (bson) => new Uri(bson.AsString)
            //);

            database = new LiteDatabase(fileName, mapper);
        }

        public void InsertOrUpdate(ICrawlerImage crawlerImage)
        {
            var collection = database.GetCollection<ICrawlerImage>("images");
            collection.Upsert(crawlerImage);
        }

        public bool Contains(string imageId)
        {
            var collection = database.GetCollection<ICrawlerImage>("images");
            return collection.Find(x => x.ImageId == imageId).Any();
        }

        public IEnumerable<string> GetImageIds()
        {
            var collection = database.GetCollection<ICrawlerImage>("images");
            return collection.FindAll().Select(x=>x.ImageId);
        }

        public IEnumerable<string> CountTags()
        {
            var collection = database.GetCollection<ICrawlerImage>("images");
            return collection.FindAll().Select(x => x.ImageId);
        }
    }
}
