using AutoTagger.Database.Standard;
using System;
using AutoTagger.Clarifai.Standard;

namespace AutoTagger.TestConsole.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("D: Graph Database Test, C: Clarifai Tagger Test, L: Lifecycle Test");
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
                case 'l':
                case 'L':
                    LifecycleTest();
                    break;
                default:
                    Console.WriteLine("No Test");
                    break;
            }

            Console.Read();
        }

        private static void LifecycleTest()
        {
            Console.WriteLine("Lifecycle Test");
            var imageLink =
                "https://images.pexels.com/photos/211707/pexels-photo-211707.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260";
            var db = new GraphDatabase();
            var tagger = new ClarifaiImageTagger();

            var tags = tagger.GetTagsForImage(imageLink);
            Console.WriteLine("Tags: " + string.Join(", ", tags));

            

            var result = db.Submit("g.V()");
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
    }
}
