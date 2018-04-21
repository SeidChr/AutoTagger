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
        private static bool dbIsUsed = false;
        private static readonly int RequestOfSameIDsLimit = 3;
        private static readonly int ConcurrentClarifaiThreadsLimit = 6;
        private static readonly int SaveLimit = 5;
        private static int SaveCounter = 0;

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
                if (taggerRunning < ConcurrentClarifaiThreadsLimit && !dbIsUsed)
                {
                    dbIsUsed = true;
                    var images = storage.GetImagesWithoutMachineTags(RequestOfSameIDsLimit);
                    dbIsUsed = false;
                    foreach (var image in images)
                    {
                        taggerRunning++;
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
            taggerRunning--;
            if (mTags.Count == 0)
                return;
            image.MachineTags = mTags;
            queue.Enqueue(image);
            SaveCounter++;
            OnFoundTags?.Invoke(image);
        }

        public static void InsertDb()
        {
            while (true)
            {
                if (!dbIsUsed && SaveCounter >= SaveLimit)
                {
                    dbIsUsed = true;
                    while (queue.TryDequeue(out IImage image))
                    {
                        storage.InsertMachineTagsWithoutSaving(image);
                        OnDbInserted?.Invoke(image);
                    }
                    storage.SaveChanges();
                    OnDbSaved?.Invoke();
                    SaveCounter = 0;
                    dbIsUsed = false;
                }
                else
                {
                    OnDbSleep?.Invoke();
                    var r = new Random();
                    Thread.Sleep(r.Next(50, 150));
                }
            }
        }

        private static void SaveDb()
        {
        }
    }
}
