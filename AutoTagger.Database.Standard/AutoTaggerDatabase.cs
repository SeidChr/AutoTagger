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

        private void CreateImage(string image)
        {
            if (!this.HasNode(image))
            {
                this.database.Submit($"g.addV('image').property('id', '{image}')");
            }
        }

        private void ConnectTag(string image, string tag)
        {
            if (!this.IsTagged(image, tag))
            {
                this.database.Submit($"g.V('{image}').addE('knows').to(g.V('{tag}'))");
            }
        }

        private void ConnectInstagramTag(string image, string tag)
        {
            if (!this.IsInstagramTagged(image, tag))
            {
                this.database.Submit($"g.V('{image}').addE('itagged').to(g.V('{tag}'))");
            }
        }

        private void CreateTag(string tag)
        {
            if (!this.HasNode(tag))
            {
                this.database.Submit($"g.addV('tag').property('id', '{tag}')");
            }
        }

        private bool IsInstagramTagged(string image, string tag)
        {
            var result = this.database.Submit($"g.V('{image}').outE('itagged').V('{tag}')");
            return result.Any();
        }

        private bool IsTagged(string image, string tag)
        {
            var result = this.database.Submit($"g.V('{image}').outE('tagged').V('{tag}')");
            return result.Any();
        }

        private bool HasNode(string image)
        {
            var result = this.database.Submit($"g.V('{image}')");
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
    }
}