using AutoTagger.Database.Standard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoTagger.Clarifai.Standard;
using AutoTagger.Contract;

namespace AutoTagger.TestConsole.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("D/G: Graph Database Test, C: Clarifai Tagger Test, R: Crawler Roundtrip");
            var key = Console.ReadKey();
            switch (key.KeyChar)
            {
                case 'c':
                case 'C':
                    ClarifaiTest();
                    break;
                case 'd':
                case 'D':
                    DatabaseTest();
                    break;
                case 'r':
                case 'R':
                    CrawlerRoudTripTest();
                    break;
                case 'g':
                case 'G':
                    var database = new AutoTaggerDatabase();
                    database.Drop();

                    database.IndertOrUpdate("schiff1", new[] { "boot", "wasser" }, new[] { "urlaub", "entspannung" });
                    database.IndertOrUpdate("boot1", new[] { "boot", "wasser" }, new[] { "urlaub", "angeln" });

                    Console.WriteLine("Graph reset and filled.s");

                    break;
                default:
                    Console.WriteLine("No Test");
                    break;
            }

            Console.Read();
        }

        private static void CrawlerRoudTripTest()
        {
            Console.WriteLine("Lifecycle Test");
            var imageLink =
                "https://images.pexels.com/photos/211707/pexels-photo-211707.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260";
            var db = new AutoTaggerDatabase() as IAutoTaggerDatabase;
            var tagger = new ClarifaiImageTagger() as ITaggingProvider;

            var maschineTags = tagger.GetTagsForImage(imageLink).ToList();
            Console.WriteLine("Tags: " + string.Join(", ", maschineTags));

            var humanoidTags = new[] {"c", "d"};

            db.IndertOrUpdate("CrawlerRoundTripTestImageId", maschineTags, humanoidTags);

            // cleanup after test
            db.Remove("CrawlerRoundTripTestImageId");
        }

        private static void DatabaseTest()
        {
            Console.WriteLine("Database Test");
            var db = new GraphDatabase();
            var result = db.Submit("g.V()");
        }

        static void ClarifaiTest()
        {
            var imageLink =
                "https://images.pexels.com/photos/211707/pexels-photo-211707.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260";
            Console.WriteLine("Clarifai Test");
            var tagger = new ClarifaiImageTagger();
            var tags = tagger.GetTagsForImage(imageLink);
            Console.WriteLine("Tags: " + string.Join(", ", tags));
        }

        //static IEnumerable<string> GetRandomInstagramTags()
        //{

        //    var tags = File.ReadLines("./instagramTags").ToList();
        //    var random = new Random();
        //    while ()
        //}
    }
}
