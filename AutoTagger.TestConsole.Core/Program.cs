namespace AutoTagger.TestConsole.Core
{
    using System;
    using System.Linq;
    using AutoTagger.Crawler.Standard;
    using AutoTagger.Crawler.Standard.V1;
    using AutoTagger.Database.Storage.Mysql;
    using AutoTagger.ImageProcessor.Standard;

    internal class Program
    {

        private static void Main(string[] args)
        {
            Console.WriteLine("" + 
                             "1: Start Crawler\n" +
                             "2: Start ImageProcessor"
                             );
            while(true)
            {
                var key = Console.ReadKey();
                Console.WriteLine("");
                switch (key.KeyChar)
                {
                    case '1':
                        Console.WriteLine("Start Crawler...");
                        StartCrawler();
                        break;

                    case '2':
                        Console.WriteLine("Start Image Processor...");
                        StartImageProcessor();
                        break;
                }
                Console.WriteLine("------------");
            }
            
        }

        private static void StartCrawler()
        {
            var db = new MysqlCrawlerStorage();
            var crawler = new CrawlerApp(db, new CrawlerV1());

            crawler.OnImageSaved += image =>
            {
                Console.WriteLine(
                    "{ \"shortcode\":\"" + image.Shortcode + "\", \"from\":\"" + image.User + "\", \"tags\": ["
                  + string.Join(", ", image.HumanoidTags.Select(x => "'" + x + "'")) + "], \"uploaded\":\""
                  + image.Uploaded + "\", " + "\"likes\":\"" + image.Likes + "\", \"follower\":\"" + image.Follower
                  + "\", \"comments\":\"" + image.Comments + "\", }");
            };
            crawler.DoCrawling(0);
        }

        private static void StartImageProcessor()
        {
            var db = new MysqlImageProcessorStorage();
            var tagger = new GCPVision();

            var imageProcessor = new ImageProcessorApp(db, tagger);
            ImageProcessorApp.OnLookingForTags += image =>
            {
                Console.WriteLine("Crawling for " + image.Id);
            };
            ImageProcessorApp.OnFoundTags += image =>
            {
                Console.WriteLine("Tags found for " + image.Id);
            };
            ImageProcessorApp.OnDbInserted += image =>
            {
                Console.WriteLine("DB Insert for " + image.Id);
            };
            ImageProcessorApp.OnDbSaved += () =>
            {
                Console.WriteLine("DB SAVED");
            };
            imageProcessor.Process();


        }
    }
}
