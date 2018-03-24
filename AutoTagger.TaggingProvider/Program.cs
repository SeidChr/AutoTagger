using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Hosting.Self;

namespace AutoTagger.TaggingProvider
{
    class Program
    {
        static void Main(string[] args)
        {
            var hostConfigs = new HostConfiguration();
            hostConfigs.UrlReservations.CreateAutomatically = true;
            var url = "http://localhost:80";
            var uri = new Uri(url);
            using (var host = new NancyHost(hostConfigs, uri))
            {
                host.Start();
                Console.WriteLine("Running AutoTaggingProvider on: " + url);
                Console.ReadLine();
            }
            Console.WriteLine("System shut down :'(");

        }
    }
}
