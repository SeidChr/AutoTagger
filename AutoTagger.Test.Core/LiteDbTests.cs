namespace AutoTagger.Test.Core
{
    using System.Linq;

    using AutoTagger.Database.Standard;

    using Xunit;

    public class LiteDbTests
    {
        [Fact]
        private static void LiteTaggingDbTest()
        {
            var db = new LiteAutoTaggerDb("taggerTest.ldb");

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
    }
}
