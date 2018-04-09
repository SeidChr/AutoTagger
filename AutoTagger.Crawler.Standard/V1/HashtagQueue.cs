using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Crawler.Standard
{
    using System.Collections.Concurrent;
    using System.Linq;

    using AutoTagger.Contract;

    class HashtagQueue<T> : ConcurrentQueue<T>
    {
        private readonly HashSet<T> processedTags;
        private readonly ShortcodeQueue<string> shortcodeQueue;

        public HashtagQueue()
        {
            this.processedTags = new HashSet<T>();
            this.shortcodeQueue = new ShortcodeQueue<string>();
        }

        public void Build(IEnumerable<T> hashTags)
        {
            foreach (var tag in hashTags)
            {
                this.Enqueue(tag);
            }
        }

        public IEnumerable<IImage> Start(Func<T, IEnumerable<string>> getShortcodes, Func<string, IImage> crawling)
        {
            while (this.TryDequeue(out T currentHashTag))
            {
                if (this.processedTags.Contains(currentHashTag))
                {
                    continue;
                }

                this.processedTags.Add(currentHashTag);
                
                var shortcodes = getShortcodes(currentHashTag);
                this.shortcodeQueue.Build(shortcodes);

                var images = this.shortcodeQueue.Process(crawling);
                foreach (var image in images)
                {
                    if (this.shortcodeQueue.IsImageProcessed(image.ImageId))
                    {
                        continue;
                    }

                    var hTags = image.HumanoidTags;
                    foreach (var tag in hTags)
                    {
                        var newTag = (T) Convert.ChangeType(tag, typeof(T));
                        this.Enqueue(newTag);
                    }

                    this.shortcodeQueue.AddImage(image.ImageId, image);
                    yield return image;

                    if (this.shortcodeQueue.IsLimitReached())
                    {
                        yield break;
                    }
                }
            }
        }

        public new void Enqueue(T tag)
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

        public bool IsTagProcessed(T tag)
        {
            return this.processedTags.Contains(tag);
        }

        public void SetLimit(int limit)
        {
            this.shortcodeQueue.SetLimit(limit);
        }

    }
}
