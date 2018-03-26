using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTagger.Contract;
using LiteDB;

namespace AutoTagger.Database.Standard
{
    public class LiteAutoTaggerDb : IAutoTaggerDatabase
    {
        private const string MachineTagsFieldName = "mashineTags";
        private const string HumanoidTagsFieldName = "humanoidTags";
        private const string ImagesCollectionName = "images";

        private readonly LiteCollection<BsonDocument> images;
        private readonly LiteDatabase database;

        public LiteAutoTaggerDb(string fileName)
        {
            database = new LiteDatabase(fileName);
            images = database.GetCollection(ImagesCollectionName);
        }

        public void Drop()
        {
            database.DropCollection(ImagesCollectionName);
        }

        public void InsertOrUpdate(string imageId, IEnumerable<string> machineTags, IEnumerable<string> humanoidTags)
        {
            images.Upsert(Bson(imageId,machineTags,humanoidTags));
        }

        private BsonDocument Bson(string imageId, IEnumerable<string> machineTags, IEnumerable<string> humanoidTags) 
            => new BsonDocument
        {
            ["_id"] = Bson(imageId),
            [MachineTagsFieldName] = Bson(machineTags),
            [HumanoidTagsFieldName] = Bson(humanoidTags),
        };

        private BsonValue Bson(string x) => new BsonValue(x);

        private BsonArray Bson(IEnumerable<string> stringEnum) => new BsonArray(stringEnum.Select(Bson).ToArray());

        private Query AnyIn(string field, IEnumerable<string> stringEnum) 
            => Query.Or(stringEnum.Select(x => Query.EQ("$."+field + ".*", x)).ToArray());

        public IEnumerable<string> FindHumanoidTags(IEnumerable<string> machineTags)
        {
            // find top 100 oldest persons aged between 20 and 30
            ////var results = col.Find(Query.And(Query.All("Age", Query.Descending), Query.Between("Age", 20, 30)), limit: 100);

            return images
                //.Find(Query.And(AnyIn(machineTags, "mashineTags"), Query.All("quality", Query.Descending)))
                .Find(AnyIn(MachineTagsFieldName, machineTags))
                .SelectMany(b => b[HumanoidTagsFieldName].AsArray.Select(ht => ht.AsString))
                .Distinct()
                .Take(30);
        }

        public void Remove(string imageId) 
            => images.Delete(new BsonValue(imageId));
    }
}
