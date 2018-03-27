namespace AutoTagger.Test.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoTagger.Clarifai.Standard;
    using AutoTagger.Crawler.Standard;
    using AutoTagger.Database.Standard;

    using Xunit;
    using Xunit.Abstractions;

    public class CrawlerTests
    {
        public CrawlerTests(ITestOutputHelper testConsole)
        {
            this.TestConsole = testConsole;
        }

        public ITestOutputHelper TestConsole { get; }

        [Fact]
        public void SpeedCrawlerTest()
        {
            var speedCrawler = new InstagramCrawlerV2();
            var goodTags = speedCrawler.Get("https://www.instagram.com/explore/tags/hamburg/");
            this.TestConsole.WriteLine(string.Join(", ", goodTags));
        }

        [Fact]
        public void CrawlerRoundtrip()
        {
            var crawler = new InstagramCrawlerV1();
            var images  = crawler.CrawlImages(10);

            var tagger = new ClarifaiImageTagger();
            var db     = new LiteAutoTaggerDb("fullImportedImages.ldb");

            IEnumerable<string> lastMTags = null;
            IEnumerable<string> lastHTags = null;

            foreach (var crawlerImage in images)
            {
                this.TestConsole.WriteLine(
                    crawlerImage.ImageId + ">L" + crawlerImage.Likes + ">C" + crawlerImage.Comments + " >>> "
                  + string.Join(", ", crawlerImage.HumanoidTags));

                var tags = tagger.GetTagsForImageUrl(crawlerImage.ImageUrl).ToList();

                lastMTags = tags;
                lastHTags = crawlerImage.HumanoidTags;

                Console.WriteLine("Tags: " + string.Join(", ", tags));
                db.InsertOrUpdate(crawlerImage.ImageId, tags, crawlerImage.HumanoidTags);
            }

            Assert.NotNull(lastMTags);
            var foundHTags = db.FindHumanoidTags(lastMTags).ToList();
            this.TestConsole.WriteLine(string.Join(", ", lastMTags) + " >>> " + string.Join(", ", foundHTags));

            // var similar    = lastHTags.Count(lastHTag => foundHTags.Contains(lastHTag));

            // Assert.True(similar > 2, "not enough similar tags");
        }

        [Fact]
        public void CrawlerTest()
        {
            var crawler = new InstagramCrawlerV1();

            // crawler.GetImageDataFromShortCode("Bgsth_jAPup");
            // crawler.GetShortCodesFromHashTag("ighamburg");
            var crawlerDb = new LiteCrawlerDb("test.ldb");
            var images    = crawler.CrawlImages(30);

            // Console.WriteLine("images: " + string.Join(", ", images.Select(x=>x.ImageId)));
            foreach (var crawlerImage in images)
            {
                this.TestConsole.WriteLine(
                    "{ \"id\":\"" + crawlerImage.ImageId + "\", \"url\":\"" + crawlerImage.ImageUrl + "\",\"tags\": ["
                  + string.Join(", ", crawlerImage.HumanoidTags.Select(x => "'" + x + "'")) + "]}");

                crawlerDb.InsertOrUpdate(crawlerImage);
            }

            this.TestConsole.WriteLine("Stored Images: " + string.Join(", ", crawlerDb.GetImageIds()));
        }

        [Fact]
        public void RandomHashtagsTest()
        {
            var crawler     = new InstagramCrawlerV1();
            var hashtagEnum = crawler.GetRandomHashTags().ToList();

            foreach (var hashtag in hashtagEnum)
            {
                this.TestConsole.WriteLine(hashtag);
            }

            Assert.True(hashtagEnum.Count > 2, "not enough random hashtags");
        }
    }
}
