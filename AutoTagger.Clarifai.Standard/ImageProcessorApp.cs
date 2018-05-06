using System;

namespace AutoTagger.ImageProcessor.Standard
{
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;

    using AutoTagger.Contract;

    public class ImageProcessorApp
    {
        private static IImageProcessorStorage storage;
        private static ITaggingProvider tagger;
        private static ConcurrentQueue<IImage> queue;
        private static ConcurrentQueue<IImage> dbSaveSueue;
        public static Action<IImage> OnLookingForTags;
        public static Action<IImage> OnFoundTags;
        public static Action<IImage> OnDbInserted;
        public static Action OnDbSleep;
        public static Action OnDbSaved;
        private static int taggerRunning;
        private static int saveCounter;
        private static readonly Random Random = new Random();

        private static readonly int FillQueueLimit = 2;
        private static readonly int ConcurrentClarifaiThreadsLimit = 5;
        private static readonly int DbSelectImagesAmount = 100;
        private static readonly int SaveLimit = 3;

        enum DbUsage
        {
            None,
            GetEntries,
            SaveThisFuckingShit,
        }
        private static DbUsage currentDbUsage;

        public ImageProcessorApp(IImageProcessorStorage db, ITaggingProvider taggingProvider)
        {
            storage = db;
            tagger = taggingProvider;
            dbSaveSueue = new ConcurrentQueue<IImage>();
            queue = new ConcurrentQueue<IImage>();
        }

        public void Process()
        {
            StartTagger();
            StartDbInsertThread();
        }

        private static void StartTagger()
        {
            new Thread(ImageProcessorApp.FillQueueThread).Start();
            new Thread(ImageProcessorApp.StartTaggerThreads).Start();
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

        private static void StartTaggerThreads()
        {
            while (true)
            {
                for (var i = taggerRunning; i < ConcurrentClarifaiThreadsLimit;i++)
                {
                    if(queue.TryDequeue(out IImage image))
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

        private static bool SetDbUsing(DbUsage newStatus)
        {
            if (currentDbUsage == DbUsage.None || currentDbUsage == newStatus)
            {
                currentDbUsage = newStatus;
                return true;
            }
            return false;
        }

        private static void StartDbInsertThread()
        {
            var dbTread = new Thread(ImageProcessorApp.InsertDb);
            dbTread.Start();
        }

        public static void DoTaggerRequest(object data)
        {
            var image = (IImage)data;
            OnLookingForTags?.Invoke(image);
            var mTags = tagger.GetTagsForImageUrl(image.LargeUrl).ToList();
            Interlocked.Decrement(ref taggerRunning);
            if (mTags.Count == 0)
                return;
            image.MachineTags = mTags;
            dbSaveSueue.Enqueue(image);
            Interlocked.Increment(ref saveCounter);
            OnFoundTags?.Invoke(image);
        }

        public static void InsertDb()
        {
            while (true)
            {
                if (saveCounter >= SaveLimit && SetDbUsing(DbUsage.SaveThisFuckingShit))
                {
                    while (dbSaveSueue.TryDequeue(out IImage image))
                    {
                        storage.InsertMachineTagsWithoutSaving(image);
                        OnDbInserted?.Invoke(image);
                    }
                    storage.DoSave();
                    OnDbSaved?.Invoke();
                    saveCounter = 0;
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
    }
}
