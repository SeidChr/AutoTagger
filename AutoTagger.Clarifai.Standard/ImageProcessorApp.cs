namespace AutoTagger.ImageProcessor.Standard
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;

    using AutoTagger.Contract;

    public class ImageProcessorApp
    {
        private const int ConcurrentClarifaiThreadsLimit = 5;

        private const int DbSelectImagesAmount = 100;

        private const int FillQueueLimit = 2;

        private const int SaveLimit = 3;

        private static readonly Random Random = new Random();

        private static DbUsage currentDbUsage;

        private static ConcurrentQueue<IImage> queue;

        private static int saveCounter;

        private static ConcurrentQueue<IImage> saveQueue;

        private static IImageProcessorStorage storage;

        private static ITaggingProvider tagger;

        private static int taggerRunning;

        public ImageProcessorApp(IImageProcessorStorage db, ITaggingProvider taggingProvider)
        {
            storage   = db;
            tagger    = taggingProvider;
            saveQueue = new ConcurrentQueue<IImage>();
            queue     = new ConcurrentQueue<IImage>();
        }

        private enum DbUsage
        {
            None,

            GetEntries,

            SaveThisFuckingShit
        }

        public static Action<IImage> OnDbInserted
        {
            get;
            set;
        }

        public static Action OnDbSaved
        {
            get;
            set;
        }

        public static Action OnDbSleep
        {
            get;
            set;
        }

        public static Action<IImage> OnFoundTags
        {
            get;
            set;
        }

        public static Action<IImage> OnLookingForTags
        {
            get;
            set;
        }

        public static void DoTaggerRequest(object data)
        {
            var image = (IImage) data;
            OnLookingForTags?.Invoke(image);

            var machineTags = tagger.GetTagsForImageUrl(image.LargeUrl).ToList();

            Interlocked.Decrement(ref taggerRunning);
            if (machineTags.Count == 0)
            {
                return;
            }

            image.MachineTags = machineTags;
            saveQueue.Enqueue(image);
            Interlocked.Increment(ref saveCounter);
            OnFoundTags?.Invoke(image);
        }

        public static void InsertDb()
        {
            while (true)
            {
                if (saveCounter >= SaveLimit && SetDbUsing(DbUsage.SaveThisFuckingShit))
                {
                    while (saveQueue.TryDequeue(out var image))
                    {
                        storage.InsertMachineTagsWithoutSaving(image);
                        OnDbInserted?.Invoke(image);
                    }

                    storage.DoSave();
                    OnDbSaved?.Invoke();
                    saveCounter    = 0;
                    currentDbUsage = DbUsage.None;
                }
                else
                {
                    OnDbSleep?.Invoke();
                    var r = new Random();
                    Thread.Sleep(r.Next(50, 150));
                }
            }
        }

        public void Process()
        {
            StartTagger();
            StartDbInsertThread();
        }

        private static void FillQueueThread()
        {
            var lastId = 0;
            while (true)
            {
                if (queue.Count <= FillQueueLimit && SetDbUsing(DbUsage.GetEntries))
                {
                    var images = storage.GetImagesWithoutMachineTags(lastId, DbSelectImagesAmount);
                    foreach (var image in images)
                    {
                        queue.Enqueue(image);
                        lastId = image.Id;
                    }

                    currentDbUsage = DbUsage.None;
                }

                Thread.Sleep(Random.Next(50, 150));
            }
        }

        private static bool SetDbUsing(DbUsage newStatus)
        {
            if (currentDbUsage != DbUsage.None && currentDbUsage != newStatus)
            {
                return false;
            }

            currentDbUsage = newStatus;
            return true;
        }

        private static void StartDbInsertThread()
        {
            var thread = new Thread(InsertDb);
            thread.Start();
        }

        private static void StartTagger()
        {
            new Thread(FillQueueThread).Start();
            new Thread(StartTaggerThreads).Start();
        }

        private static void StartTaggerThreads()
        {
            while (true)
            {
                for (var i = taggerRunning; i < ConcurrentClarifaiThreadsLimit; i++)
                {
                    if (queue.TryDequeue(out var image))
                    {
                        Interlocked.Increment(ref taggerRunning);
                        var taggerThread = new Thread(DoTaggerRequest);
                        taggerThread.Start(image);
                    }
                    else
                    {
                        break;
                    }
                }

                Thread.Sleep(Random.Next(50, 150));
            }
        }
    }
}
