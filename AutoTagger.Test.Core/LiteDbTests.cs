namespace AutoTagger.Test.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoTagger.Contract;
    using AutoTagger.Crawler.Standard;
    using AutoTagger.Database.Storage.AutoTagger;
    using LiteDB;
    using Xunit;

    public class LiteDbTests
    {
        [Fact]
        public void LiteDbQueryTests()
        {
            var database       = new LiteDatabase("queryTest.ldb");
            var testCollection = database.GetCollection("test");

            testCollection.Upsert(new BsonDocument
            {
                ["_id"] = "1",
                ["a"]   = this.Bson(1, 2, 3, 4),
            });

            testCollection.Upsert(new BsonDocument
            {
                ["_id"] = "2",
                ["a"]   = this.Bson(3, 4, 5, 6)
            });

            // var expr  = new BsonExpression("SUM($.Items[*].Unity * $.Items[*].Price)");
            // var total = expr.Execute(doc, true).First().AsDecimal;
            var result = testCollection.Find(Query.LT("SUM($.a[*])", 11)).ToList();
            Assert.Single(result);
            Assert.Collection(
                result,
                resultDoc =>
                {
                    Assert.Equal(10, resultDoc["a"].AsArray.Aggregate(0, (i, v) => v.AsInt32 + i));
                });

            database.DropCollection("test");
        }

        [Fact]
        public void LiteTaggingDbTest()
        {
            var db = new LiteDbAutoTaggerStorage("taggerTest.ldb");

            db.InsertOrUpdate("iA", new[] { "mA", "mB", "mC" }, new[] { "hA", "hB" });
            db.InsertOrUpdate("iB", new[] { "mA", "mB", "mD" }, new[] { "hA", "hC" });
            db.InsertOrUpdate("iC", new[] { "mA", "mD", "mE" }, new[] { "hC", "hD" });
            db.InsertOrUpdate("iD", new[] { "mX", "mY", "mZ" }, new[] { "hX", "hY" });
            db.InsertOrUpdate("iE", new[] { "mA", "mG", "mU" }, new[] { "hA", "hF" });

            var (debug, tags) = db.FindHumanoidTags(new List<IMTag> { new MTag{Name="mA" }, new MTag { Name = "mB" }, new MTag { Name = "mC" } });

            Assert.Contains("hC", tags);
            Assert.DoesNotContain("hX", tags);

            db.Drop();
        }

        private BsonValue Bson<T>(T x)
        {
            return new BsonValue(x);
        }

        private BsonArray Bson<T>(params T[] x)
        {
            return new BsonArray(x.Select(this.Bson));
        }
    }
}
