using AutoTagger.Database.Standard;
using System;

namespace AutoTagger.TestConsole.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("G: Graph Database Test, C: Clarifai Tagger Test");
            var key = Console.ReadKey();
            switch (key.KeyChar)
            {
                case 'c':
                case 'C':
                    Console.WriteLine("Clarifai Test");
                    break;
                case 'd':
                case 'D':
                    Console.WriteLine("DB Test");
                    var db = new GraphDatabase();
                    var result = db.Submit("g.V()");
                    break;
                default:
                    Console.WriteLine("No Test");
                    break;
            }

            Console.Read();
        }
    }
}
