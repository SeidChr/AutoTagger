namespace AutoTagger.TestConsole.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AutoTagger.Clarifai.Standard;
    using AutoTagger.Database.Context.AutoTagger;

    internal class Program
    {
        public static void ImportInstagramTags()
        {
            var linkWithTags = File.ReadLines("./imageLinks.txt").ToList();
            foreach (var linkWithTag in linkWithTags)
            {
                var splitted     = linkWithTag.Split(',');
                var link         = splitted.First();
                var humanoidTags = splitted.Skip(1).First().Split('/');

                var tagger = new ClarifaiImageTagger();
                var repository = new CosmosAutoTaggerStorage();

                var machineTags = tagger.GetTagsForImageUrl(link);

                repository.InsertOrUpdate(link, machineTags, humanoidTags);
            }
        }

        private static void ClarifaiTest()
        {
            var imageLink =
                "https://images.pexels.com/photos/211707/pexels-photo-211707.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260";

            Console.WriteLine("Clarifai Test");

            var tagger = new ClarifaiImageTagger();
            var tags   = tagger.GetTagsForImageUrl(imageLink);

            Console.WriteLine("Tags: " + string.Join(", ", tags));
        }

        private static void DatabaseReadTest()
        {
            var repository = new CosmosAutoTaggerStorage();
            var instagramTags = repository.FindHumanoidTags(new[] { "boot", "fisch", "egal" });

            Console.WriteLine("You should use the following instagram tags:");
            foreach (var tag in instagramTags)
            {
                Console.WriteLine(tag);
            }
        }

        private static void DatabaseInsertTest()
        {
            // Console.WriteLine("Database Test");
            // var db = new CosmosGraphDatabase();
            // var result = db.Submit("g.V()");
            var repository = new CosmosAutoTaggerStorage();
            //context.Drop();

            repository.InsertOrUpdate("schiff1", new[] { "boot", "wasser" }, new[] { "urlaub", "entspannung" });

            repository.InsertOrUpdate("boot1", new[] { "boot", "fisch" }, new[] { "urlaub", "angeln" });

            Console.WriteLine("Graph reset and filled.s");
        }

        private static IEnumerable<string> GetRandomInstagramTags(int seed)
        {
            var result = new List<string>();
            var tags   = File.ReadLines("./instagramTags.txt").ToList();
            var random = new Random(seed);
            while (result.Count < 30)
            {
                var index = random.Next(tags.Count - 1);
                var item  = tags[index];
                tags.RemoveAt(index);
                result.Add(item);
            }

            return result;
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("1: Database Insert \n" +
                              "2: Database Read\n" +
                              "3: Clarifai Tagger\n" +
                              "4: Import Testdata from File"
                              );
            while(true)
            {
                var key = Console.ReadKey();
                switch (key.KeyChar)
                {
                    case '1':
                        DatabaseInsertTest();
                        break;

                    case '2':
                        DatabaseReadTest();
                        break;

                    case '3':
                        ClarifaiTest();
                        break;

                    case '4':
                        ImportInstagramTags();
                        break;
                }
                Console.WriteLine("------------");
            }
            
        }
    }
}
