namespace AutoTagger.Test.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoTagger.Contract;
    using AutoTagger.Crawler.Standard;
    using AutoTagger.Database.Storage.AutoTagger;
    using AutoTagger.Database.Storage.Crawler;
    using Xunit;

    public class MysqlTests
    {
        [Fact]
        public void MysqlInsertPhoto()
        {
            // Arrange
            var crawlerDb = new MysqlCrawlerStorage();
            crawlerDb.GetAllHumanoidTags();
            var image = new Image
            {
                Comments = 10,
                Follower = 99,
                Following = 150,
                Posts = 42,
                HumanoidTags = new List<string> { "catlove", "instabeach", "hamburg" },
                MachineTags = new List<string> { "cat", "beach", "city" },
                LargeUrl = "content.com/pic/ab12xy67laaaarge",
                ThumbUrl = "content.com/pic/ab12xthump",
                Shortcode = "ab12xy67",
                Likes = 1337,
                User = "DarioDomi",
                Uploaded = DateTime.Now
            };

            // Act

            crawlerDb.InsertOrUpdate(image);

            // Assert
            Assert.NotEmpty(image.Shortcode);
        }
        [Fact]
        public void MysqlInsertITag()
        {
            // Arrange
            var crawlerDb = new MysqlCrawlerStorage();
            crawlerDb.GetAllHumanoidTags();
            var name = "Altona";
            var posts = 14;
            var humanoidTag = new HumanoidTag {Name = name, Posts = posts};

            // Act
            crawlerDb.InsertOrUpdateHumaniodTag(humanoidTag);

            // Assert
            Assert.True(true);
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
