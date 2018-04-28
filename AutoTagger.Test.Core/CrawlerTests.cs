namespace AutoTagger.Test.Core
{
    using System.Collections.Concurrent;
    using System.Linq;
    using AutoTagger.Contract;
    using AutoTagger.Crawler.Standard;
    using AutoTagger.Crawler.Standard.V1;
    using AutoTagger.Crawler.Standard.V1.Crawler;
    using AutoTagger.Database.Storage.Mysql;
    using Xunit;
    using Xunit.Abstractions;

    public class CrawlerTests
    {
        private readonly ICrawlerStorage db;

        public CrawlerTests(ITestOutputHelper testConsole)
        {
            this.testConsole = testConsole;
            //this.db = new LiteCrawlerStorage("test.ldb");
            this.db = new MysqlCrawlerStorage();
        }

        private ITestOutputHelper testConsole { get; }

        [Fact]
        public void CrawlerTest()
        {
            var crawler = new CrawlerApp(this.db, new CrawlerV1());

            //crawler.DoCrawling(1, "gratidão");
            //crawler.DoCrawling(1);
            crawler.DoCrawling(0);
        }

        [Fact]
        public void RandomHashtagsTest()
        {
            var crawler  = new RandomTagsCrawler();
            var hashtags = crawler.Parse().ToList();

            foreach (var hashtag in hashtags)
            {
                this.testConsole.WriteLine(hashtag);
            }

            Assert.True(hashtags.Count > 2, "not enough random hashtags");
        }

        [Fact]
        public void SpeedCrawlerTest()
        {
            var limit = 10;

            var speedCrawler = new CrawlerV2();
            var queue        = new ConcurrentQueue<string>();
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
    }
}
