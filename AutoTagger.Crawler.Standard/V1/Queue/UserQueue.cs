using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Crawler.Standard.V1
{
    using System.Collections.Concurrent;
    using System.Linq;

    using AutoTagger.Contract;

    class UserQueue<T> : ConcurrentQueue<T>
    {
        private readonly HashSet<T> processed;

        public UserQueue()
        {
            this.processed = new HashSet<T>();
        }

        public IEnumerable<IImage> Process(
            Func<string, IEnumerable<IImage>> userPageCrawling)
        {
            while (this.TryDequeue(out T userNameAsT))
            {
                if (IsProcessed(userNameAsT))
                {
                    continue;
                }

                var userName = (string)Convert.ChangeType(userNameAsT, typeof(string));
                this.AddProcessed(userNameAsT);

                var images = userPageCrawling(userName);
                foreach (var image in images)
                {
                    yield return image;
                }
            }
        }

        public new void Enqueue(T shortCode)
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
    }
}
