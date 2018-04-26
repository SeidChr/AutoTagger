using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Clarifai.Standard
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
        public static Action<IImage> OnLookingForTags;
        public static Action<IImage> OnFoundTags;
        public static Action<IImage> OnDbInserted;
        public static Action OnDbSleep;
        public static Action OnDbSaved;
        private static int taggerRunning = 0;
        private static readonly int RequestOfSameIDsLimit = 3;
        private static readonly int ConcurrentClarifaiThreadsLimit = 9;
        private static readonly int SaveLimit = 5;
        private static int SaveCounter = 0;
        enum DbUsage
        {
            None,
            GetEntries,
            SaveThisFuckingShit,
        }
        private static DbUsage currentDbUsage;

        public ImageProcessorApp(IImageProcessorStorage db)
        {
            storage = db;
            tagger = new ClarifaiImageTagger();
            queue = new ConcurrentQueue<IImage>();
        }

        public void Process()
        {
            StartTagger();
            StartDbInsertThread();
        }

        private static void StartTagger()
        {
            var taggerThread = new Thread(ImageProcessorApp.TaggerThread);
            taggerThread.Start();
        }

        private static void TaggerThread()
        {
            while (true)
            {
                if (taggerRunning < ConcurrentClarifaiThreadsLimit && SetDbUsing(DbUsage.GetEntries))
                {
                    var images = storage.GetImagesWithoutMachineTags(RequestOfSameIDsLimit);
                    currentDbUsage = DbUsage.None;
                    foreach (var image in images)
                    {
                        Interlocked.Increment(ref taggerRunning);
                        var clarifaiThread = new Thread(ImageProcessorApp.GetData);
                        clarifaiThread.Start(image);
                    }
                }
                else
                {
                    var r = new Random();
                    Thread.Sleep(r.Next(50, 150));
                }
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

        public static void GetData(object data)
        {
            IImage image = (IImage)data;
            OnLookingForTags?.Invoke(image);
            var mTags = tagger.GetTagsForImageUrl(image.LargeUrl).ToList();
            Interlocked.Decrement(ref taggerRunning);
            if (mTags.Count == 0)
                return;
            image.MachineTags = mTags;
            queue.Enqueue(image);
            Interlocked.Increment(ref SaveCounter);
            OnFoundTags?.Invoke(image);
        }

        public static void InsertDb()
        {
            while (true)
            {
                if (SaveCounter >= SaveLimit && SetDbUsing(DbUsage.SaveThisFuckingShit))
                {
                    while (queue.TryDequeue(out IImage image))
                    {
                        storage.InsertMachineTagsWithoutSaving(image);
                        OnDbInserted?.Invoke(image);
                    }
                    storage.DoSave();
                    OnDbSaved?.Invoke();
                    SaveCounter = 0;
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
