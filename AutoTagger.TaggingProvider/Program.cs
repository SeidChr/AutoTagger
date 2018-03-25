using System;
using AutoTagger.Clarifai.Standard;
using Newtonsoft.Json;

namespace AutoTagger.TaggingProvider
{
    class Program
    {
        static void Main(string[] args)
        {
            var imageTagger = new ClarifaiImageTagger();
            Console.WriteLine("Insert a link:");

            while (true)
            {
                var link = Console.ReadLine();
                Console.WriteLine("inserted: " + link);

                var tags = imageTagger.GetTagsForImageUrl(link);
                var output = JsonConvert.SerializeObject(tags);
                Console.WriteLine(output);
            }


        }
    }
}
