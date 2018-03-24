using System;
using System.Linq;
using AutoTagger.Clarifai.Standard;
using AutoTagger.Contract;
using AutoTagger.Database.Standard;
using Newtonsoft.Json;

namespace AutoTagger.UserInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            var tagger = new ClarifaiImageTagger() as ITaggingProvider;
            Console.WriteLine("Hello, please insert an URL");

            while (true)
            {
                var link = Console.ReadLine();
                Console.WriteLine("inserted: '" + link + "'");

                var maschineTags = tagger.GetTagsForImage(link).ToList();
                Console.WriteLine("Contains maschineTags: " + String.Join(", ", maschineTags));

                var db = new AutoTaggerDatabase() as IAutoTaggerDatabase;

                try
                {
                    var instagramTags = db.FindInstagramTags(maschineTags).ToList();
                    var output = JsonConvert.SerializeObject(instagramTags);
                    Console.WriteLine(output);
                }
                catch (NotImplementedException e)
                {

                }

            }
        }
    }
}
