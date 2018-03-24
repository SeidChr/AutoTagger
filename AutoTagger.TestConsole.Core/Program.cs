using AutoTagger.Database.Standard;
using System;

namespace AutoTagger.TestConsole.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new GraphDatabase();
            var result = db.Submit("g.V()");
            Console.Read();
        }
    }
}
