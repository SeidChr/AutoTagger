namespace AutoTagger.Database.Standard
{
    using System;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    public class TaggedDatabase : ITaggedDatabase
    {
        private readonly GraphDatabase database;

        public TaggedDatabase()
        {
            this.database = new GraphDatabase();
        }

        public void Add(string image, string[] automaticTags, string[] instagramTags)
        {
            if (!this.HasNode(image))
            {
                this.database.Submit($"g.addV('image').property('id', '{image}')");
            }

            foreach (var tag in automaticTags)
            {
                if (!this.HasNode(tag))
                {
                    this.database.Submit($"g.addV('tag').property('id', '{tag}')");
                }

                if (!this.IsTagged(image, tag))
                {
                    this.database.Submit($"g.V('{image}').addE('knows').to(g.V('{tag}'))");
                }
            }

            foreach (var tag in instagramTags)
            {
                if (!this.HasNode(tag))
                {
                    this.database.Submit($"g.addV('itag').property('id', '{tag}')");
                }

                if (!this.IsInstagramTagged(image, tag))
                {
                    this.database.Submit($"g.V('{image}').addE('itagged').to(g.V('{tag}'))");
                }
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

        public string[] FindInstagramTags(string[] automaticTags)
        {
            throw new NotImplementedException();
        }
    }
}