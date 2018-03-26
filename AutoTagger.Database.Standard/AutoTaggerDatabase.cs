using System.Collections.Generic;

namespace AutoTagger.Database.Standard
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using Newtonsoft.Json.Linq;
    using AutoTagger.Contract;

    public class AutoTaggerDatabase : IAutoTaggerDatabase
    {
        private readonly GraphDatabase database;

        public AutoTaggerDatabase()
        {
            this.database = new GraphDatabase();
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

        private void CreateImage(string imageId)
        {
            if (!this.HasNode(imageId))
            {
                this.database.Submit($"g.addV('image').property('id', '{CleanInput(imageId)}')");
            }
        }

        private void ConnectTag(string imageId, string tag)
        {
            if (!this.IsTagged(imageId, tag))
            {
                this.database.Submit($"g.V('{CleanInput(imageId)}').addE('tagged').to(g.V('{CleanInput(tag)}'))");
            }
        }

        private void ConnectInstagramTag(string imageId, string tag)
        {
            if (!this.IsInstagramTagged(imageId, tag))
            {
                this.database.Submit($"g.V('{CleanInput(imageId)}').addE('itagged').to(g.V('{CleanInput(tag)}'))");
            }
        }

        private void CreateTag(string tag)
        {
            if (!this.HasNode(tag))
            {
                this.database.Submit($"g.addV('tag').property('id', '{CleanInput(tag)}')");
            }
        }

        private bool IsInstagramTagged(string imageId, string tag)
        {
            var result = this.database.Submit($"g.V('{CleanInput(imageId)}').out('itagged').has('id','{CleanInput(tag)}')");
            return result.Any();
        }

        private bool IsTagged(string imageId, string tag)
        {
            var result = this.database.Submit($"g.V('{CleanInput(imageId)}').out('tagged').has('id','{CleanInput(tag)}')");
            return result.Any();
        }

        private bool HasNode(string imageId)
        {
            var result = this.database.Submit($"g.V('{CleanInput(imageId)}')");
            return result.Any();
        }

        public void Drop()
        {
            this.database.Submit($"g.V().drop()");
        }

        public IEnumerable<string> FindHumanoidTags(IEnumerable<string> machineTags)
        {
            var tagString = machineTags.Select(CleanInput).Aggregate(string.Empty, (i, j) => i + "','" + j).Trim('\'', ',');


            var result = this.database.Submit(
                $"g.V().hasLabel('image').order().by(out('tagged').has('id',within('{tagString}')).count().as('count'), decr).limit(10).out('itagged').dedup().limit(10)");

            return result.Select(i => (string)i["id"]);
        }

        public void Remove(string imageId)
        {
            this.database.Submit($"g.V('{imageId}').drop()");
        }

        private static string CleanInput(string input)
        {
            return Regex.Replace(input, @"[^\w\.@-]", string.Empty);
        }
    }
}