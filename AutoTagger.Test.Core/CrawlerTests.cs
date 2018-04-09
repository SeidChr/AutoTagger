namespace AutoTagger.Test.Core
{
    using System;
    using System.Collections.Concurrent;
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
        public void SpeedCrawlerTest()
        {
            var speedCrawler = new CrawlerV2();
            var queue = new ConcurrentQueue<string>();
            queue.Enqueue("hamburg");
            speedCrawler.FoundImage += i =>
            {
                this.TestConsole.WriteLine(i.ToString());
                foreach (var humanoidTag in i.HumanoidTags)
                {
                    if (!queue.Contains(humanoidTag))
                    {
                        queue.Enqueue(humanoidTag);
                    }
                }
            };

            int parsed = 0;
            while (parsed < 10 && queue.TryDequeue(out var hashtag))
            {
                parsed++;

                this.TestConsole.WriteLine("Queue Size: " + queue.Count);
                this.TestConsole.WriteLine("Parsing HashTag: #" + hashtag);
                speedCrawler.ParseHashTagPage(hashtag);
            }

            this.TestConsole.WriteLine("Remained Queue: " + string.Join(", ", queue));
            this.TestConsole.WriteLine("HashTag Pages Parsed: " + parsed);
        }

        [Fact]
        public void CrawlerRoundtrip()
        {
            var crawler = new CrawlerV1();
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
            var crawler = new CrawlerV1();

            var images    = crawler.DoCrawling(1, "travel");  

            //Console.WriteLine("images: " + string.Join(", ", images.Select(x => x.ImageId)));

            foreach (var image in images)
            {
                this.TestConsole.WriteLine(
                    "{ \"id\":\"" + image.ImageId + "\", \"url\":\"" + image.ImageUrl + "\",\"tags\": ["
                  + string.Join(", ", image.HumanoidTags.Select(x => "'" + x + "'")) + "]}");

                db.InsertOrUpdate(image);
            }

            //this.TestConsole.WriteLine("Stored Images: " + string.Join(", ", crawlerDb.GetImageIds()));
        }

        [Fact]
        public void RandomHashtagsTest()
        {
            var crawler     = new CrawlingJob();
            var hashtagEnum = crawler.GetRandomHashtags().ToList();

            foreach (var hashtag in hashtagEnum)
            {
                this.TestConsole.WriteLine(hashtag);
            }

            Assert.True(hashtagEnum.Count > 2, "not enough random hashtags");
        }
    }
}
