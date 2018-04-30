namespace AutoTagger.Database.Storage.AutoTagger
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using global::AutoTagger.Contract;
    using global::AutoTagger.Database.Helper;

    public class CosmosAutoTaggerStorage : IAutoTaggerStorage
    {
        private readonly CosmosGraphDatabase database;

        public CosmosAutoTaggerStorage()
        {
            this.database = new CosmosGraphDatabase();
        }

        public void Drop()
        {
            this.database.Submit($"g.V().drop()");
        }

        public (string debug, IEnumerable<string> htags) FindHumanoidTags(List<IMTag> machineTags)
        {
            var mtags = machineTags.Select(x => x.Name);
            var tagString = mtags.Select(CleanInput).Aggregate(string.Empty, (i, j) => i + "','" + j)
                .Trim('\'', ',');

            var result = this.database.Submit(
                $"g.V().hasLabel('image').order().by(out('tagged').has('id',within('{tagString}')).count().as('count'), decr).limit(10).out('itagged').dedup().limit(10)");

            return ("", result.Select(i => (string)i["id"]));
        }

        public void Log(string source, string data)
        {
            throw new System.NotImplementedException();
        }

        public void InsertOrUpdate(string imageId, IEnumerable<string> machineTags, IEnumerable<string> humanoidTags)
        {
            this.CreateImage(imageId);

            foreach (var tag in machineTags)
            {
                this.CreateTag(tag);

                this.ConnectTag(imageId, tag);
            }

            foreach (var tag in humanoidTags)
            {
                this.CreateTag(tag);

                this.ConnectInstagramTag(imageId, tag);
            }
        }

        public void Remove(string imageId)
        {
            this.database.Submit($"g.V('{imageId}').drop()");
        }

        public void Dispose()
        {
        }

        private static string CleanInput(string input)
        {
            return Regex.Replace(input, @"[^\w\.@-]", string.Empty);
        }

        private void ConnectInstagramTag(string imageId, string tag)
        {
            if (!this.IsInstagramTagged(imageId, tag))
            {
                this.database.Submit($"g.V('{CleanInput(imageId)}').addE('itagged').to(g.V('{CleanInput(tag)}'))");
            }
        }

        private void ConnectTag(string imageId, string tag)
        {
            if (!this.IsTagged(imageId, tag))
            {
                this.database.Submit($"g.V('{CleanInput(imageId)}').addE('tagged').to(g.V('{CleanInput(tag)}'))");
            }
        }

        private void CreateImage(string imageId)
        {
            if (!this.HasNode(imageId))
            {
                this.database.Submit($"g.addV('image').property('id', '{CleanInput(imageId)}')");
            }
        }

        private void CreateTag(string tag)
        {
            if (!this.HasNode(tag))
            {
                this.database.Submit($"g.addV('tag').property('id', '{CleanInput(tag)}')");
            }
        }

        private bool HasNode(string imageId)
        {
            var result = this.database
                .Submit($"g.V('{CleanInput(imageId)}')");

            return result.Any();
        }

        private bool IsInstagramTagged(string imageId, string tag)
        {
            var result = this.database
                .Submit($"g.V('{CleanInput(imageId)}').out('itagged').has('id','{CleanInput(tag)}')");

            return result.Any();
        }

        private bool IsTagged(string imageId, string tag)
        {
            var result = this.database
                .Submit($"g.V('{CleanInput(imageId)}').out('tagged').has('id','{CleanInput(tag)}')");

            return result.Any();
        }
    }
}
