namespace AutoTagger.Crawler.Standard.V1.Queue
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using AutoTagger.Contract;

    internal class HashtagQueue<T> : ConcurrentQueue<T>
    {
        private readonly HashSet<T> processed;

        private readonly ShortcodeQueue<string> shortcodeQueue;

        public HashtagQueue()
        {
            this.processed      = new HashSet<T>();
            this.shortcodeQueue = new ShortcodeQueue<string>();
        }

        public event Action<T> OnHashtagFound;

        public void AddProcessed(IEnumerable<T> tags)
        {
            foreach (var tag in tags)
            {
                this.processed.Add(tag);
            }
        }

        public void Build(IEnumerable<T> hashTags)
        {
            foreach (var tag in hashTags)
            {
                this.Enqueue(tag);
            }
        }

        public IEnumerable<IImage> Process(
            Func<T, (int, List<string>)> shortcodesCrawling,
            Func<string, string> imagePageCrawling,
            Func<string, IEnumerable<IImage>> userPageCrawling)
        {
            while (this.TryDequeue(out var currentTag))
            {
                if (this.IsTagProcessed(currentTag))
                {
                    continue;
                }

                var (amountPosts, shortcodes) = shortcodesCrawling(currentTag);

                SetAmountOfPosts(currentTag, amountPosts);
                this.AddProcessed(currentTag);
                this.HashtagFound(currentTag);

                this.shortcodeQueue.Build(shortcodes);

                var images = this.shortcodeQueue.Process(imagePageCrawling, userPageCrawling);

                foreach (var image in images)
                {
                    // limit check
                    if (image == null)
                    {
                        yield break;
                    }

                    var humanoidTags = image.HumanoidTags;
                    foreach (var humanoidTagname in humanoidTags)
                    {
                        var newHTag = new HumanoidTag
                        {
                            Name = humanoidTagname
                        };

                        var newHTagAsT = (T)Convert.ChangeType(newHTag, typeof(HumanoidTag));
                        this.Enqueue(newHTagAsT);
                    }

                    yield return image;
                }
            }
        }

        public void SetLimit(int limit)
        {
            this.shortcodeQueue.SetLimit(limit);
        }

        private static void SetAmountOfPosts(T currentTag, int amountPosts)
        {
            var humanoidTagType = currentTag.GetType();
            var pinfo    = humanoidTagType.GetProperty("Posts");
            pinfo.SetValue(currentTag, amountPosts, null);
        }

        private void AddProcessed(T tag)
        {
            this.processed.Add(tag);
        }

        private bool Contains(T checkingTag)
        {
            var checkingHTag = (HumanoidTag)Convert.ChangeType(checkingTag, typeof(HumanoidTag));
            var exists = this.FirstOrDefault(
                htag => ((HumanoidTag)Convert.ChangeType(htag, typeof(HumanoidTag))).Name == checkingHTag.Name);
            return exists != null;
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

        private void HashtagFound(T tag)
        {
            this.OnHashtagFound?.Invoke(tag);
        }

        private bool IsTagProcessed(T checkingTag)
        {
            var checkingHTag = (HumanoidTag)Convert.ChangeType(checkingTag, typeof(HumanoidTag));
            foreach (var htag in this.processed)
            {
                var newHTag = (HumanoidTag)Convert.ChangeType(htag, typeof(HumanoidTag));
                if (newHTag.Name == checkingHTag.Name && newHTag.Posts != 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
