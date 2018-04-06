namespace AutoTagger.Test.Core
{
    using System;
    using System.Collections.Generic;
    using AutoTagger.Database.Standard.Context.AutoTagger;
    using AutoTagger.Database.Standard.Context.Crawler;
    using Xunit;
    using Image = Crawler.Standard.Image;

    public class MysqlTests
    {
        [Fact]
        public void WhenCrawlerInsert_ThenDbShouldSaveCorrectly()
        {
            // Arrange
            var crawlerDb = new MysqlCrawlerStorage();
            //var autoTaggerDb = new MysqlAutoTaggerStorage();
            var image = new Image()
            {
                Comments = 10,
                Follower = 200,
                HumanoidTags = new List<string> { "cat", "beach", "city" },
                ImageUrl = "testImageUrl",
                Likes = 1337
            };

            // Act
            crawlerDb.InsertOrUpdate(image);
            //var res = autoTaggerDb.FindHumanoidTags(new List<String>() { "cat" });

            // Assert
            Assert.NotEmpty(image.ImageId);
        }
    }
}
