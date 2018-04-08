namespace AutoTagger.Test.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoTagger.Clarifai.Standard;
    using AutoTagger.Contract;
    using AutoTagger.Crawler.Standard;
    using AutoTagger.Database.Context.Crawler;

    using Xunit;
    using Xunit.Abstractions;

    public class CrawlerTests
    {
        private readonly ICrawlerStorage db;
        private ITestOutputHelper TestConsole { get; }

        public CrawlerTests(ITestOutputHelper testConsole)
        {
            TestConsole = testConsole;
            //var db = new LiteCrawlerStorage("test.ldb");
            db = new MysqlCrawlerStorage();
        }

        [Fact]
        public void CrawlerRoundtrip()
        {
            var crawler = new Crawler();
            var images  = crawler.DoCrawling(1);

            var tagger = new ClarifaiImageTagger();

            IEnumerable<string> lastMTags = null;

            foreach (var crawlerImage in images)
            {
                this.TestConsole.WriteLine(
                    crawlerImage.ImageId + ">L" + crawlerImage.Likes + ">C" + crawlerImage.Comments + " >>> "
                  + string.Join(", ", crawlerImage.HumanoidTags));

                var tags = tagger.GetTagsForImageUrl(crawlerImage.ImageUrl).ToList();

                lastMTags = tags;

                Console.WriteLine("Tags: " + string.Join(", ", tags));
                db.InsertOrUpdate(crawlerImage);
            }

            Assert.NotNull(lastMTags);
            //var foundHTags = db.FindHumanoidTags(lastMTags).ToList();
            //this.TestConsole.WriteLine(string.Join(", ", lastMTags) + " >>> " + string.Join(", ", foundHTags));

            // var similar    = lastHTags.Count(lastHTag => foundHTags.Contains(lastHTag));

            // Assert.True(similar > 2, "not enough similar tags");
        }

        [Fact]
        public void CrawlerTest()
        {
            var crawler = new Crawler();

            var images    = crawler.DoCrawling(2);

            Console.WriteLine("images: " + string.Join(", ", images.Select(x => x.ImageId)));

            foreach (var crawlerImage in images)
            {
                this.TestConsole.WriteLine(
                    "{ \"id\":\"" + crawlerImage.ImageId + "\", \"url\":\"" + crawlerImage.ImageUrl + "\",\"tags\": ["
                  + string.Join(", ", crawlerImage.HumanoidTags.Select(x => "'" + x + "'")) + "]}");

                db.InsertOrUpdate(crawlerImage);
            }

            //this.TestConsole.WriteLine("Stored Images: " + string.Join(", ", crawlerDb.GetImageIds()));
        }

        [Fact]
        public void RandomHashtagsTest()
        {
            var crawler     = new Crawler();
            var hashtagEnum = crawler.GetRandomHashTags().ToList();

            foreach (var hashtag in hashtagEnum)
            {
                this.TestConsole.WriteLine(hashtag);
            }

            Assert.True(hashtagEnum.Count > 2, "not enough random hashtags");
        }
    }
}
