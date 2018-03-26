namespace AutoTagger.Database.Standard
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoTagger.Contract;

    using LiteDB;

    public class LiteAutoTaggerDb : IAutoTaggerDatabase
    {
        private const string HumanoidTagsFieldName = "humanoidTags";

        private const string ImagesCollectionName = "images";

        private const string MachineTagsFieldName = "mashineTags";

        private readonly LiteDatabase database;

        private readonly LiteCollection<BsonDocument> images;

        public LiteAutoTaggerDb(string fileName)
        {
            this.database = new LiteDatabase(fileName);
            this.images   = this.database.GetCollection(ImagesCollectionName);
        }

        public void Drop()
        {
            this.database.DropCollection(ImagesCollectionName);
        }

        public IEnumerable<string> FindHumanoidTags(IEnumerable<string> machineTags)
        {
            // find top 100 oldest persons aged between 20 and 30
            ////var results = col.Find(Query.And(Query.All("Age", Query.Descending), Query.Between("Age", 20, 30)), limit: 100);
            // .Find(Query.And(AnyIn(machineTags, "mashineTags"), Query.All("quality", Query.Descending)))
            return this.images
                .Find(this.AnyIn(MachineTagsFieldName, machineTags))
                .SelectMany(b => b[HumanoidTagsFieldName].AsArray.Select(ht => ht.AsString))
                .Distinct()
                .Take(30);
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
