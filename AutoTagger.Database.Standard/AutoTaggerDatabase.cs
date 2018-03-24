using System.Collections.Generic;

namespace AutoTagger.Database.Standard
{
    using System;
    using System.Linq;

    using Newtonsoft.Json.Linq;
    using AutoTagger.Contract;

    public class AutoTaggerDatabase : IAutoTaggerDatabase
    {
        private readonly GraphDatabase database;

        public AutoTaggerDatabase()
        {
            this.database = new GraphDatabase();
        }

        public void IndertOrUpdate(string imageId, IEnumerable<string> maschineTags, IEnumerable<string> humanoidTags)
        {
            this.CreateImage(imageId);

            foreach (var tag in maschineTags)
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
                this.database.Submit($"g.addV('imageId').property('id', '{imageId}')");
            }
        }

        private void ConnectTag(string imageId, string tag)
        {
            if (!this.IsTagged(imageId, tag))
            {
                this.database.Submit($"g.V('{imageId}').addE('knows').to(g.V('{tag}'))");
            }
        }

        private void ConnectInstagramTag(string imageId, string tag)
        {
            if (!this.IsInstagramTagged(imageId, tag))
            {
                this.database.Submit($"g.V('{imageId}').addE('itagged').to(g.V('{tag}'))");
            }
        }

        private void CreateTag(string tag)
        {
            if (!this.HasNode(tag))
            {
                this.database.Submit($"g.addV('tag').property('id', '{tag}')");
            }
        }

        private bool IsInstagramTagged(string imageId, string tag)
        {
            var result = this.database.Submit($"g.V('{imageId}').outE('itagged').V('{tag}')");
            return result.Any();
        }

        private bool IsTagged(string imageId, string tag)
        {
            var result = this.database.Submit($"g.V('{imageId}').outE('tagged').V('{tag}')");
            return result.Any();
        }

        private bool HasNode(string imageId)
        {
            var result = this.database.Submit($"g.V('{imageId}')");
            return result.Any();
        }

        public void Drop()
        {
            this.database.Submit($"g.V().drop()");
        }

        public IEnumerable<string> FindInstagramTags(IEnumerable<string> maschineTags)
        {
            throw new NotImplementedException();
        }

        public void Remove(string imageId)
        {
            this.database.Submit($"g.V('{imageId}').drop()");
        }
    }
}