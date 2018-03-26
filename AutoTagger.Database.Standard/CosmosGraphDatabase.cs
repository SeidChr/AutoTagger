namespace AutoTagger.Database.Standard
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gremlin.Net.Driver;
    using Gremlin.Net.Structure.IO.GraphSON;

    public class CosmosGraphDatabase
    {
        private const string AuthKey =
            "YVHgjHTuHXDLEWQ3TcxpsN65MHYN8tuNWcLVkVIYCWV9rPjnRKlvbj0aWBDPG8gvXBNoxlBDsvSDCYgdLIseJA==";

        private const string Collection = "images";

        private const string Database = "images";

        private const string Hostname = "lab-image-tagging.gremlin.cosmosdb.azure.com";

        private const int Port = 443;

        private readonly GremlinClient client;

        // private static Dictionary<string, string> gremlinQueries =
        // new Dictionary<string, string>
        // {
        // { "Cleanup", "g.V().drop()" },
        // {
        // "AddVertex 1",
        // "g.addV('person').property('id', 'thomas').property('firstName', 'Thomas').property('age', 44)"
        // },
        // {
        // "AddVertex 2",
        // "g.addV('person').property('id', 'mary').property('firstName', 'Mary').property('lastName', 'Andersen').property('age', 39)"
        // },
        // {
        // "AddVertex 3",
        // "g.addV('person').property('id', 'ben').property('firstName', 'Ben').property('lastName', 'Miller')"
        // },
        // {
        // "AddVertex 4",
        // "g.addV('person').property('id', 'robin').property('firstName', 'Robin').property('lastName', 'Wakefield')"
        // },
        // { "AddEdge 1", "g.V('thomas').addE('knows').to(g.V('mary'))" },
        // { "AddEdge 2", "g.V('thomas').addE('knows').to(g.V('ben'))" },
        // { "AddEdge 3", "g.V('ben').addE('knows').to(g.V('robin'))" },
        // { "UpdateVertex", "g.V('thomas').property('age', 44)" },
        // { "CountVertices", "g.V().count()" },
        // { "Filter Range", "g.V().hasLabel('person').has('age', gt(40))" },
        // { "Project", "g.V().hasLabel('person').values('firstName')" },
        // { "Sort", "g.V().hasLabel('person').order().by('firstName', decr)" },
        // { "Traverse", "g.V('thomas').out('knows').hasLabel('person')" },
        // {
        // "Traverse 2x",
        // "g.V('thomas').out('knows').hasLabel('person').out('knows').hasLabel('person')"
        // },
        // {
        // "Loop",
        // "g.V('thomas').repeat(out()).until(has('id', 'robin')).path()"
        // },
        // {
        // "DropEdge",
        // "g.V('thomas').outE('knows').where(inV().has('id', 'mary')).drop()"
        // },
        // { "CountEdges", "g.E().count()" },
        // { "DropVertex", "g.V('thomas').drop()" },
        // };
        public CosmosGraphDatabase()
        {
            this.client = CreateClient();
        }

        public IReadOnlyCollection<dynamic> Submit(string script)
        {
            var task = this.SubmitAsync(script);

            try
            {
                task.Wait();
            }
            catch (Exception)
            {
                Thread.Sleep(50000);
                try
                {
                    var retryTask = this.SubmitAsync(script);
                    retryTask.Wait();
                    return retryTask.Result;
                }
                catch (Exception)
                {
                    Console.WriteLine("Graph Query Error unrecoverable for script " + script);
                    return new ReadOnlyCollection<dynamic>(Enumerable.Empty<dynamic>().ToList());
                }
            }

            return task.Result;
        }

        public async Task<IReadOnlyCollection<dynamic>> SubmitAsync(string query)
        {
            return await this.client.SubmitAsync<dynamic>(query);
        }

        private static GremlinClient CreateClient()
        {
            var server = CreateServer();

            var client = new GremlinClient(
                server,
                new GraphSON2Reader(),
                new GraphSON2Writer(),
                GremlinClient.GraphSON2MimeType);

            return client;
        }

        private static GremlinServer CreateServer()
        {
            var server = new GremlinServer(
                Hostname,
                Port,
                true,
                "/dbs/" + Database + "/colls/" + Collection,
                AuthKey);

            return server;
        }
    }
}
