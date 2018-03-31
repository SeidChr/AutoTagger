namespace AutoTagger.Test.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoTagger.Database.Standard;

    using LiteDB;

    using Xunit;

    public class LiteDbTests
    {
        ////[Fact]
        ////public void LiteDbQueryTest()
        ////{
        ////    using (var database = this.Database<BsonDocument>("queryTest.ldb", "test"))
        ////    {
        ////        var c = database.Collection;

        ////        //// count how many elements of "a" overlap with emelents of match
        ////        //// a.Where(e=>m.Contains(e)).Count()
        ////        //// Where(e=>m.Contains(e))
        ////        //// $.Books[SUBSTRING(LOWER(@.Title), 0, 1) = 'j']
        ////        //// IIF(<condition>, <ifTrue>, <ifFalse>)
        ////        //// $.a[IIF(,1,0)=1]

        ////        ////c.Upsert(new BsonDocument { ["_id"] = "1", ["a"] = this.Bson(1, 2, 3, 4) });
        ////        ////c.Upsert(new BsonDocument { ["_id"] = "2", ["a"] = this.Bson(3, 4, 5, 6) });

        ////        ////var match = this.Bson(2, 3, 4);

        ////        ////// var expr  = new BsonExpression("SUM($.Items[*].Unity * $.Items[*].Price)");
        ////        ////// var total = expr.Execute(doc, true).First().AsDecimal;
        ////        ////var result = c.Find(Query.LT("SUM($.a[*])", 11)).ToList();
        ////        ////Assert.Single(result);
        ////        ////Assert.Collection(
        ////        ////    result,
        ////        ////    resultDoc =>
        ////        ////    {
        ////        ////        Assert.Equal(10, resultDoc["a"].AsArray.Aggregate(0, (i, v) => v.AsInt32 + i));
        ////        ////    });
        ////    }
        ////}

        [Fact]
        public void LiteDbQuerySumTest()
        {
            using (var database = this.Database<BsonDocument>("queryTest.ldb", "test"))
            {
                var c = database.Collection;

                c.Upsert(new BsonDocument { ["_id"] = "1", ["a"] = this.Bson(1, 2, 3, 4) });
                c.Upsert(new BsonDocument { ["_id"] = "2", ["a"] = this.Bson(3, 4, 5, 6) });

                // var expr  = new BsonExpression("SUM($.Items[*].Unity * $.Items[*].Price)");
                // var total = expr.Execute(doc, true).First().AsDecimal;
                var result = c.Find(Query.LT("SUM($.a[*])", 11)).ToList();
                Assert.Single(result);
                Assert.Collection(
                    result,
                    resultDoc =>
                    {
                        Assert.Equal(10, resultDoc["a"].AsArray.Aggregate(0, (i, v) => v.AsInt32 + i));
                    });
            }
        }

        [Fact]
        public void LiteTaggingDbTestA()
        {
            var db = new LiteDbAutoTaggerStorage("taggerTest.ldb");

            db.InsertOrUpdate("iA", new[] { "mA", "mB", "mC" }, new[] { "hA", "hB" });
            db.InsertOrUpdate("iB", new[] { "mA", "mB", "mD" }, new[] { "hA", "hC" });
            db.InsertOrUpdate("iC", new[] { "mA", "mD", "mE" }, new[] { "hC", "hD" });
            db.InsertOrUpdate("iD", new[] { "mX", "mY", "mZ" }, new[] { "hX", "hY" });
            db.InsertOrUpdate("iE", new[] { "mA", "mG", "mU" }, new[] { "hA", "hF" });

            var tags = db.FindHumanoidTags(new[] { "mA", "mB", "mC" }).ToList();

            Assert.Contains("hC", tags);
            Assert.DoesNotContain("hX", tags);

            db.Drop();
        }

        [Fact]
        public void LiteTaggingDbTestB()
        {
            var db = new LiteDbAutoTaggerStorage("taggerTest.ldb");

            db.InsertOrUpdate("iA", new[] { "mA", "mB", "mC" }, new[] { "hA", "hB" });
            db.InsertOrUpdate("iB", new[] { "mA", "mB", "mD" }, new[] { "hA", "hC" });
            db.InsertOrUpdate("iC", new[] { "mA", "mD", "mE" }, new[] { "hC", "hD" });
            db.InsertOrUpdate("iD", new[] { "mX", "mY", "mZ" }, new[] { "hX", "hY" });
            db.InsertOrUpdate("iE", new[] { "mA", "mG", "mU" }, new[] { "hA", "hF" });

            var tags = db.FindHumanoidTags(new[] { "mA", "mB", "mC" }).ToList();

            Assert.Contains("hC", tags);
            Assert.DoesNotContain("hX", tags);

            db.Drop();
        }

        private Query AnyIn(string field, BsonArray array)
        {
            return Query.Or(array.Select(x => Query.EQ("$." + field + "[*]", x)).ToArray());
        }

        private BsonValue Bson<T>(T x)
        {
            return new BsonValue(x);
        }

        private BsonArray Bson<T>(params T[] x)
        {
            return new BsonArray(x.Select(this.Bson));
        }

        private TestCollectionWrapper<T> Database<T>(string file, string collection)
        {
            return new TestCollectionWrapper<T>(file, collection);
        }

        private class TestCollectionWrapper<T> : IDisposable
        {
            public readonly LiteCollection<T> Collection;

            private readonly LiteDatabase database;

            public TestCollectionWrapper(string file, string collection)
            {
                this.database   = new LiteDatabase(file);
                this.Collection = this.database.GetCollection<T>(collection);
            }

            public void Dispose()
            {
                var name = this.Collection.Name;
                this.database.DropCollection(this.Collection.Name);
            }
        }
    }
}
