namespace AutoTagger.Crawler.Standard.V1
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using AutoTagger.Contract;

    class HashtagQueue<T> : ConcurrentQueue<T>
    {
        private readonly HashSet<T> processed;
        private readonly ShortcodeQueue<string> shortcodeQueue;

        public HashtagQueue()
        {
            this.processed = new HashSet<T>();
            this.shortcodeQueue = new ShortcodeQueue<string>();
        }

        public void Build(IEnumerable<T> hashTags)
        {
            foreach (var tag in hashTags)
            {
                this.Enqueue(tag);
            }
        }

        public IEnumerable<IImage> Process(Func<T, IEnumerable<string>> shortcodesCrawling,
                                           Func<string, string> imagePageCrawling,
                                           Func<string, IEnumerable<IImage>> userPageCrawling
            )
        {
            while (this.TryDequeue(out T currentHashTag))
            {
                if (this.processed.Contains(currentHashTag))
                {
                    continue;
                }

                this.AddProcessed(currentHashTag);
                
                var shortcodes = shortcodesCrawling(currentHashTag);
                this.shortcodeQueue.Build(shortcodes);

                var images = this.shortcodeQueue.Process(imagePageCrawling, userPageCrawling);

                foreach (var image in images)
                {
                    var hTags = image.HumanoidTags;
                    foreach (var tag in hTags)
                    {
                        var newTag = (T)Convert.ChangeType(tag, typeof(T));
                        this.Enqueue(newTag);
                    }

                    yield return image;
                }
            }
        }

        private new void Enqueue(T tag)
        {
            if (tag == null)
            {
                return;
            }
            if (this.IsTagProcessed(tag))
            {
                return;
            }
            if (this.Contains(tag))
            {
                return;
            }
            base.Enqueue(tag);
        }

        private bool IsTagProcessed(T tag)
        {
            return this.processed.Contains(tag);
        }

        private void AddProcessed(T value)
        {
            this.processed.Add(value);
        }

        public void SetLimit(int limit)
        {
            this.shortcodeQueue.SetLimit(limit);
        }

    }
}
