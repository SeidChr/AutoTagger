using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Crawler.Standard
{
    using System.Collections.Concurrent;
    using System.Linq;

    using AutoTagger.Contract;

    class ShortcodeQueue<T> : ConcurrentQueue<T>
    {
        private readonly Dictionary<T, IImage> processedImages;
        private int limit;

        public ShortcodeQueue()
        {
            this.processedImages = new Dictionary<T, IImage>();
            this.limit = -1;
        }

        public void Build(IEnumerable<T> shortcodes)
        {
            foreach (var tag in shortcodes)
            {
                this.Enqueue(tag);
            }
        }

        public IEnumerable<IImage> Process(Func<T, IImage> crawlingFunc)
        {
            while (this.TryDequeue(out T currentShortcode))
            {
                if (this.processedImages.ContainsKey(currentShortcode))
                {
                    continue;
                }

                var image = crawlingFunc(currentShortcode);
                yield return image;
            }
        }

        public new void Enqueue(T shortCode)
        {
            if (shortCode == null)
            {
                return;
            }
            if (this.IsImageProcessed(shortCode))
            {
                return;
            }
            if (this.Contains(shortCode))
            {
                return;
            }
            base.Enqueue(shortCode);
        }

        public bool IsImageProcessed(T shortCode)
        {
            return this.processedImages.ContainsKey(shortCode);
        }

        public void AddImage(T shortCode, IImage image)
        {
            this.processedImages.Add(shortCode, image);
        }

        public void SetLimit(int limit)
        {
            this.limit = limit;
        }

        public bool IsLimitReached()
        {
            return this.processedImages.Count >= this.limit;
        }

    }
}
