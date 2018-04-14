namespace AutoTagger.Test.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using AutoTagger.Clarifai.Standard;
    using AutoTagger.Contract;
    using AutoTagger.Crawler.Standard;
    using AutoTagger.Crawler.Standard.V1;
    using AutoTagger.Crawler.Standard.V1.Crawler;
    using AutoTagger.Database.Context.Crawler;

    using Xunit;
    using Xunit.Abstractions;

    public class CrawlerTests
    {
        private readonly ICrawlerStorage db;
        private ITestOutputHelper testConsole { get; }

        public CrawlerTests(ITestOutputHelper testConsole)
        {
            this.testConsole = testConsole;
            //this.db = new LiteCrawlerStorage("test.ldb");
            this.db = new MysqlCrawlerStorage();
        }

        [Fact]
        public void SpeedCrawlerTest()
        {
            var limit = 10;

            var speedCrawler = new CrawlerV2();
            var queue = new ConcurrentQueue<string>();
            queue.Enqueue("hamburg");
            speedCrawler.FoundImage += i =>
            {
                this.testConsole.WriteLine(i.ToString());
                foreach (var humanoidTag in i.HumanoidTags)
                {
                    if (!queue.Contains(humanoidTag))
                    {
                        queue.Enqueue(humanoidTag);
                    }
                }
            };

            while (limit > 0 && queue.TryDequeue(out var hashtag))
            {
                limit--;

                this.testConsole.WriteLine("Queue Size: " + queue.Count);
                this.testConsole.WriteLine("Parsing HashTag: #" + hashtag);
                speedCrawler.ParseHashTagPage(hashtag);
            }

            this.testConsole.WriteLine("Remained Queue: " + string.Join(", ", queue));
        }

        [Fact]
        public void CrawlerRoundtrip()
        {
            var crawler = new CrawlerV1();
            var images  = crawler.DoCrawling(20);

            var tagger = new ClarifaiImageTagger();

            IEnumerable<string> lastMTags = null;

            foreach (var image in images)
            {
                this.testConsole.WriteLine(
                    image.Shortcode + ">L" + image.Likes + ">C" + image.CommentCount + " >>> "
                  + string.Join(", ", image.HumanoidTags));

                image.MachineTags = tagger.GetTagsForImageUrl(image.Url).ToList();
                lastMTags = image.MachineTags;

                Console.WriteLine("Tags: " + string.Join(", ", image.MachineTags));
                db.InsertOrUpdate(image);
            }

            Assert.NotNull(lastMTags);
            //var foundHTags = db.FindHumanoidTags(lastMTags).ToList();
            //this.testConsole.WriteLine(string.Join(", ", lastMTags) + " >>> " + string.Join(", ", foundHTags));

            // var similar    = lastHTags.Count(lastHTag => foundHTags.Contains(lastHTag));

            // Assert.True(similar > 2, "not enough similar tags");
        }

        [Fact]
        public void CrawlerTest()
        {
            var crawler = new CrawlerV1();

            //var images = crawler.DoCrawling(3, "travel");  
            var images = crawler.DoCrawling(0);  

            foreach (var image in images)
            {
                //this.testConsole.WriteLine(
                //    "{ \"id\":\"" + image.Shortcode + "\", \"url\":\"" + image.Url + "\",\"tags\": ["
                //  + string.Join(", ", image.HumanoidTags.Select(x => "'" + x + "'")) + "]}");

                db.InsertOrUpdate(image);
            }
        }

        [Fact]
        public void RandomHashtagsTest()
        {
            var crawler     = new RandomTagsCrawler();
            var hashtags = crawler.Parse().ToList();

            foreach (var hashtag in hashtags)
            {
                this.testConsole.WriteLine(hashtag);
            }

            Assert.True(hashtags.Count > 2, "not enough random hashtags");
        }
    }
}
