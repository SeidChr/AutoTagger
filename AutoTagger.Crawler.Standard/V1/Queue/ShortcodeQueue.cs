using System;
using System.Collections.Generic;

namespace AutoTagger.Crawler.Standard.V1
{
    using System.Collections.Concurrent;
    using System.Linq;

    using AutoTagger.Contract;

    class ShortcodeQueue<T> : ConcurrentQueue<T>
    {
        private readonly HashSet<T> processed;
        private int limit;
        private readonly UserQueue<string> userQueue;

        public ShortcodeQueue()
        {
            this.processed = new HashSet<T>();
            this.limit = -1;
            this.userQueue = new UserQueue<string>();
        }

        public void Build(IEnumerable<T> shortcodes)
        {
            foreach (var tag in shortcodes)
            {
                this.Enqueue(tag);
            }
        }

        public IEnumerable<IImage> Process(Func<T, string> imagePageCrawling,
                                           Func<string, IEnumerable<IImage>> userPageCrawling
            )
        {
            while (this.TryDequeue(out T currentShortcode))
            {
                if (this.IsProcessed(currentShortcode))
                {
                    continue;
                }

                var userName = imagePageCrawling(currentShortcode);
                if (String.IsNullOrEmpty(userName))
                {
                    continue;
                }
                userQueue.Enqueue(userName);
                var images = userQueue.Process(userPageCrawling);

                foreach (var image in images)
                {
                    var imageId = (T)Convert.ChangeType(image.Shortcode, typeof(T));
                    if (this.IsProcessed(imageId))
                    {
                        continue;
                    }

                    this.AddProcessed(imageId);
                    yield return image;

                    if (this.IsLimitReached())
                    {
                        yield break;
                    }
                }
            }
        }

        private new void Enqueue(T shortCode)
        {
            if (shortCode == null)
            {
                return;
            }
            if (this.IsProcessed(shortCode))
            {
                return;
            }
            if (this.Contains(shortCode))
            {
                return;
            }
            base.Enqueue(shortCode);
        }

        private bool IsProcessed(T value)
        {
            return this.processed.Contains(value);
        }

        private void AddProcessed(T value)
        {
            this.processed.Add(value);
        }

        public void SetLimit(int limit)
        {
            this.limit = limit;
        }

        private bool IsLimitReached()
        {
            return this.processed.Count >= this.limit;
        }

    }
}
