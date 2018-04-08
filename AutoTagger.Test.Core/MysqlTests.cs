namespace AutoTagger.Test.Core
{
    using System;
    using System.Collections.Generic;
    using AutoTagger.Crawler.Standard;
    using AutoTagger.Database.Context.AutoTagger;
    using AutoTagger.Database.Context.Crawler;
    using Xunit;

    public class MysqlTests
    {
        [Fact]
        public void WhenCrawlerInsert()
        {
            // Arrange
            var crawlerDb = new MysqlCrawlerStorage();
            var image = new Image()
            {
                Comments = 10,
                Follower = 200,
                HumanoidTags = new List<string> { "catlove", "instabeach", "hamburg" },
                MachineTags = new List<string> { "cat", "beach", "city" },
                ImageUrl = "content.com/pic/ab12xy67",
                ImageId = "ab12xy67",
                InstaUrl = "instagram.com/abc123",
                Likes = 1337
            };

            // Act
            crawlerDb.InsertOrUpdate(image);

            // Assert
            Assert.NotEmpty(image.ImageId);
        }

        [Fact]
        public void WhenGettingAllPhotos()
        {
            // Arrange
            var mysql = new MysqlAutoTaggerStorage();

            // Act
            var photos = mysql.GetAllPhotos();

            // Assert
            foreach (var photo in photos)
            {
                Console.WriteLine(photo.Id);
            }
            Assert.NotEmpty(photos);
        }
    }
}
