
namespace AutoTagger.Database.Storage.AutoTagger
{
    using System.Collections.Generic;
    using System.Linq;
    using global::AutoTagger.Contract;
    using LiteDB;

    public class LiteDbAutoTaggerStorage : IAutoTaggerStorage
    {
        private const string HumanoidTagsFieldName = "humanoidTags";

        private const string ImagesCollectionName = "images";

        private const string MachineTagsFieldName = "mashineTags";

        private readonly LiteDatabase database;

        private readonly LiteCollection<BsonDocument> images;

        public LiteDbAutoTaggerStorage(string fileName)
        {
            this.database = new LiteDatabase(fileName);
            this.images   = this.database.GetCollection(ImagesCollectionName);
        }

        public void Drop()
        {
            this.database.DropCollection(ImagesCollectionName);
        }

        public (string debug, IEnumerable<string> htags) FindHumanoidTags(List<IMTag> machineTags)
        {
            var mtags = machineTags.Select(x => x.Name);
            // find top 100 oldest persons aged between 20 and 30
            ////var results = col.Find(Query.And(Query.All("Age", Query.Descending), Query.Between("Age", 20, 30)), limit: 100);
            // .Find(Query.And(AnyIn(machineTags, "mashineTags"), Query.All("quality", Query.Descending)))
            var htags = this.images
                .Find(this.AnyIn(MachineTagsFieldName, mtags))
                .SelectMany(b => b[HumanoidTagsFieldName].AsArray.Select(ht => ht.AsString))
                .Distinct()
                .Take(30);
            return ("", htags);
        }

        public void Log(string source, string data)
        {
            throw new System.NotImplementedException();
        }

        public void InsertOrUpdate(string imageId, IEnumerable<string> machineTags, IEnumerable<string> humanoidTags)
        {
            this.images.Upsert(this.Bson(imageId, machineTags, humanoidTags));
        }

        public void Remove(string imageId)
        {
            this.images.Delete(new BsonValue(imageId));
        }

        private Query AnyIn(string field, IEnumerable<string> stringEnum)
        {
            return Query.Or(stringEnum.Select(x => Query.EQ("$." + field + "[*]", x)).ToArray());
        }

        private BsonDocument Bson(string imageId, IEnumerable<string> machineTags, IEnumerable<string> humanoidTags)
        {
            return new BsonDocument
            {
                ["_id"]                 = this.Bson(imageId),
                [MachineTagsFieldName]  = this.Bson(machineTags),
                [HumanoidTagsFieldName] = this.Bson(humanoidTags)
            };
        }

        private BsonValue Bson(string x)
        {
            return new BsonValue(x);
        }

        private BsonArray Bson(IEnumerable<string> stringEnum)
        {
            return new BsonArray(stringEnum.Select(this.Bson).ToArray());
        }
    }
}
