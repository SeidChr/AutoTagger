namespace AutoTagger.Crawler.Standard.V1.Queue
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using AutoTagger.Contract;

    internal class ShortcodeQueue<T> : ConcurrentQueue<T>
    {
        private readonly HashSet<T> processed;

        private readonly UserQueue<string> userQueue;

        private int limit;

        public ShortcodeQueue()
        {
            this.processed = new HashSet<T>();
            this.limit     = -1;
            this.userQueue = new UserQueue<string>();
        }

        public void Build(IEnumerable<T> shortcodes)
        {
            foreach (var tag in shortcodes)
            {
                this.Enqueue(tag);
            }
        }

        public IEnumerable<IImage> Process(
            Func<T, string> imagePageCrawling,
            Func<string, IEnumerable<IImage>> userPageCrawling)
        {
            while (this.TryDequeue(out var currentShortcode))
            {
                if (this.IsProcessed(currentShortcode))
                {
                    continue;
                }

                if (this.IsLimitReached())
                {
                    yield return null;
                }

                var userName = imagePageCrawling(currentShortcode);
                if (string.IsNullOrEmpty(userName))
                {
                    continue;
                }

                this.userQueue.Enqueue(userName);
                var images = this.userQueue.Process(userPageCrawling);

                foreach (var image in images)
                {
                    if (this.IsLimitReached())
                    {
                        yield return null;
                    }

                    var shortcode = (T) Convert.ChangeType(image.Shortcode, typeof(T));
                    this.AddProcessed(shortcode);
                    yield return image;
                }
            }
        }

        public void SetLimit(int limit)
        {
            this.limit = limit > 0
                ? limit
                : -1;
        }

        private void AddProcessed(T value)
        {
            this.processed.Add(value);
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

        private bool IsLimitReached()
        {
            if (this.limit == -1)
            {
                return false;
            }

            return this.processed.Count >= this.limit;
        }

        private bool IsProcessed(T value)
        {
            return this.processed.Contains(value);
        }
    }
}
