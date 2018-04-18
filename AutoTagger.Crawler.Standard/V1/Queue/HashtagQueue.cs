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
                if (this.IsTagProcessed(currentHashTag))
                {
                    continue;
                }

                this.AddProcessed(currentHashTag);
                
                var shortcodes = shortcodesCrawling(currentHashTag);
                this.shortcodeQueue.Build(shortcodes);

                var images = this.shortcodeQueue.Process(imagePageCrawling, userPageCrawling);

                foreach (var image in images)
                {
                    // limit check
                    if (image == null)
                    {
                        yield break;
                    }

                    var hTagNames = image.HumanoidTags;
                    foreach (var hTagname in hTagNames)
                    {
                        var newHTag = new HumanoidTag();
                        newHTag.Name = hTagname;
                        var newHTagAsT = (T)Convert.ChangeType(newHTag, typeof(HumanoidTag));
                        this.Enqueue(newHTagAsT);
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

        private bool IsTagProcessed(T checkingTag)
        {
            var checkingHTag = (HumanoidTag)Convert.ChangeType(checkingTag, typeof(HumanoidTag));
            var query = from htag in this.processed
                        where ((HumanoidTag)Convert.ChangeType(htag, typeof(HumanoidTag))).Name == checkingHTag.Name
                        select htag;
            var exists = query.FirstOrDefault();
            return exists != null;
        }

        private bool Contains(T checkingTag)
        {
            var checkingHTag = (HumanoidTag)Convert.ChangeType(checkingTag, typeof(HumanoidTag));
            var exists = this.FirstOrDefault(htag => ((HumanoidTag)Convert.ChangeType(htag, typeof(HumanoidTag))).Name == checkingHTag.Name);
            return exists != null;
        }

        public void AddProcessed(IEnumerable<T> tags)
        {
            foreach (var tag in tags)
            {
                this.processed.Add(tag);
            }
        }

        private void AddProcessed(T tag)
        {
            this.processed.Add(tag);
        }

        public void SetLimit(int limit)
        {
            this.shortcodeQueue.SetLimit(limit);
        }

    }
}
