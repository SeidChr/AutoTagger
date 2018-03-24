namespace AutoTagger.Database.Standard
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoTagger.Contract;
    using Gremlin.Net.Driver;

    public class GraphDatabase : IAutoTaggerDatabase
    {
        private static string hostname = "lab-image-tagging.documents.azure.com";
        private static int port = 443;
        private static string authKey =
            "YVHgjHTuHXDLEWQ3TcxpsN65MHYN8tuNWcLVkVIYCWV9rPjnRKlvbj0aWBDPG8gvXBNoxlBDsvSDCYgdLIseJA==";
        private static string database = "images";
        private static string collection = "images";

        private GremlinClient Client { get; }

        public static Dictionary<string, string> GremlinQueries =
            new Dictionary<string, string>
                {
                    { "Cleanup", "g.V().drop()" },
                    {
                        "AddVertex 1",
                        "g.addV('person').property('id', 'thomas').property('firstName', 'Thomas').property('age', 44)"
                    },
                    {
                        "AddVertex 2",
                        "g.addV('person').property('id', 'mary').property('firstName', 'Mary').property('lastName', 'Andersen').property('age', 39)"
                    },
                    {
                        "AddVertex 3",
                        "g.addV('person').property('id', 'ben').property('firstName', 'Ben').property('lastName', 'Miller')"
                    },
                    {
                        "AddVertex 4",
                        "g.addV('person').property('id', 'robin').property('firstName', 'Robin').property('lastName', 'Wakefield')"
                    },
                    { "AddEdge 1", "g.V('thomas').addE('knows').to(g.V('mary'))" },
                    { "AddEdge 2", "g.V('thomas').addE('knows').to(g.V('ben'))" },
                    { "AddEdge 3", "g.V('ben').addE('knows').to(g.V('robin'))" },
                    { "UpdateVertex", "g.V('thomas').property('age', 44)" },
                    { "CountVertices", "g.V().count()" },
                    { "Filter Range", "g.V().hasLabel('person').has('age', gt(40))" },
                    { "Project", "g.V().hasLabel('person').values('firstName')" },
                    { "Sort", "g.V().hasLabel('person').order().by('firstName', decr)" },
                    { "Traverse", "g.V('thomas').out('knows').hasLabel('person')" },
                    {
                        "Traverse 2x",
                        "g.V('thomas').out('knows').hasLabel('person').out('knows').hasLabel('person')"
                    },
                    {
                        "Loop",
                        "g.V('thomas').repeat(out()).until(has('id', 'robin')).path()"
                    },
                    {
                        "DropEdge",
                        "g.V('thomas').outE('knows').where(inV().has('id', 'mary')).drop()"
                    },
                    { "CountEdges", "g.E().count()" },
                    { "DropVertex", "g.V('thomas').drop()" },
                };

        public GraphDatabase()
        {
            this.Client = this.CreateClient();
        }

        public async Task<IReadOnlyCollection<dynamic>> Submit(string script)
        {
            var result = await Client.SubmitAsync<dynamic>(script);
            return result;
        }

        private GremlinServer CreateServer()
        {
            var server = new GremlinServer(
                hostname,
                port,
                true,
                "/dbs/" + database + "/colls/" + collection,
                authKey);
            return server;
        }

        private GremlinClient CreateClient()
        {
            var server = this.CreateServer();
            var client = new GremlinClient(server);
            return client;
        }
    }
}