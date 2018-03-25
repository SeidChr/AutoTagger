using AutoTagger.Database.Standard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoTagger.Clarifai.Standard;
using AutoTagger.Contract;

namespace AutoTagger.TestConsole.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("D/F: Graph Database Test, C: Clarifai Tagger Test, R: Crawler Roundtrip, I: Import Testdata from File, X: crawler test");
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

                case 'i':
                case 'I':
                    ImportInstagramTags();
                    break;

                case 'f':
                case 'F':
                    DatabaseReadTest();
                    break;

                case 'x':
                case 'X':
                    CrawlerTest();
                    break;

                default:
                    Console.WriteLine("No Test");
                    break;
            }

            Console.ReadLine();
        }

        private static void CrawlerRoudTripTest()
        {
            Console.WriteLine("Lifecycle Test");
            var imageLink =
                "https://images.pexels.com/photos/211707/pexels-photo-211707.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260";
            var db = new AutoTaggerDatabase() as IAutoTaggerDatabase;
            var tagger = new ClarifaiImageTagger() as ITaggingProvider;

            var machineTags = tagger.GetTagsForImageUrl(imageLink).ToList();
            Console.WriteLine("Mashine Tags: " + string.Join(", ", machineTags));

            var humanoidTags = GetRandomInstagramTags(imageLink.Length).ToList();
            Console.WriteLine("Human Tags: " + string.Join(", ", humanoidTags));

            db.IndertOrUpdate("CrawlerRoundTripTestImageId", machineTags, humanoidTags);

            // cleanup after test
            //db.Remove("CrawlerRoundTripTestImageId");
        }

        private static void CrawlerTest()
        {
            /// Bgsth_jAPup
            var crawler = new Crawler.Standard.Crawler();
            //crawler.GetImageDataFromShortCode("Bgsth_jAPup");
            //crawler.GetShortCodesFromHashTag("ighamburg");
            var images = crawler.GetImagesFromHashTag("ighamburg", 1000);
            //Console.WriteLine("images: " + string.Join(", ", images.Select(x=>x.ImageId)));
            var tagger = new ClarifaiImageTagger();
            var db = new AutoTaggerDatabase();

            foreach (var crawlerImage in images)
            {
                Console.WriteLine("Adding image "+crawlerImage.ImageId + " to db");
                var tags = tagger.GetTagsForImageUrl(crawlerImage.ImageUrl).ToList();
                Console.WriteLine("Tags: " + string.Join(", ", tags));
                db.IndertOrUpdate(crawlerImage.ImageId, tags, crawlerImage.HumanoidTags);
            }
        }

        private static void DatabaseTest()
        {
            //Console.WriteLine("Database Test");
            //var db = new GraphDatabase();
            //var result = db.Submit("g.V()");

            var database = new AutoTaggerDatabase();
            database.Drop();

            database.IndertOrUpdate("schiff1", new[] { "boot", "wasser" }, new[] { "urlaub", "entspannung" });
            database.IndertOrUpdate("boot1", new[] { "boot", "fisch" }, new[] { "urlaub", "angeln" });

            Console.WriteLine("Graph reset and filled.s");
        }

        private static void DatabaseReadTest()
        {
            var database = new AutoTaggerDatabase();
            var instagramTags = database.FindInstagramTags(new[] { "boot", "fisch", "egal" });
            Console.WriteLine("You should use the following instagram tags:");
            foreach (var tag in instagramTags)
            {
                Console.WriteLine(tag);
            }
        }

        static void ClarifaiTest()
        {
            var imageLink =
                "https://images.pexels.com/photos/211707/pexels-photo-211707.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260";
            Console.WriteLine("Clarifai Test");
            var tagger = new ClarifaiImageTagger();
            var tags = tagger.GetTagsForImageUrl(imageLink);
            Console.WriteLine("Tags: " + string.Join(", ", tags));
        }

        static IEnumerable<string> GetRandomInstagramTags(int seed)
        {
            List<string> result = new List<string>();
            var tags = File.ReadLines("./instagramTags.txt").ToList();
            var random = new Random(seed);
            while (result.Count < 30)
            {
                var index = random.Next(tags.Count - 1);
                var item = tags[index];
                tags.RemoveAt(index);
                result.Add(item);
            }

            return result;
        }

        public static void ImportInstagramTags()
        {
            List<string> result = new List<string>();
            var linkWithTags = File.ReadLines("./imageLinks.txt").ToList();
            foreach (var linkWithTag in linkWithTags)
            {
                var splitted = linkWithTag.Split(',');
                var link = splitted.First();
                var humanoidTags = splitted.Skip(1).First().Split('/');

                var tagger = new ClarifaiImageTagger();
                var autoTaggerDatabase = new AutoTaggerDatabase();

                var machineTags = tagger.GetTagsForImageUrl(link);

                autoTaggerDatabase.IndertOrUpdate(link, machineTags, humanoidTags);
            }

            //while (result.Count < 30)
            //{
            //    var index = random.Next(tags.Count - 1);
            //    var item = tags[index];
            //    tags.RemoveAt(index);
            //    result.Add(item);
            //}

            //return result;
        }
    }
}
