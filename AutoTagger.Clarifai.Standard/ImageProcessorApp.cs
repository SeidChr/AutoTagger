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
        private static int taggerRunning = 0;
        private static bool dbIsUsed = false;
        private static readonly int requestOfSameIDsLimit = 3;
        private static readonly int concurrentStarting = 10;

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
                if (taggerRunning <= concurrentStarting && !dbIsUsed)
                {
                    dbIsUsed = true;
                    var images = storage.GetImagesWithoutMachineTags(requestOfSameIDsLimit);
                    foreach (var image in images)
                    {
                        var clarifaiThread = new Thread(ImageProcessorApp.GetData);
                        clarifaiThread.Start(image);
                        taggerRunning++;
                    }
                    dbIsUsed = false;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        private static void StartDbInsertThread()
        {
            var dbTread = new Thread(ImageProcessorApp.InsertData);
            dbTread.Start();
        }

        public static void GetData(object data)
        {
            IImage image = (IImage)data;
            OnLookingForTags?.Invoke(image);
            image.MachineTags = tagger.GetTagsForImageUrl(image.LargeUrl).ToList();
            queue.Enqueue(image);
            taggerRunning--;
            OnFoundTags?.Invoke(image);
        }

        public static void InsertData()
        {
            while (true)
            {
                if (!dbIsUsed && queue.TryDequeue(out IImage image))
                {
                    dbIsUsed = true;
                    storage.InsertMachineTags(image);
                    dbIsUsed = false;
                    OnDbInserted?.Invoke(image);
                }
                else
                {
                    OnDbSleep?.Invoke();
                    Thread.Sleep(100);
                }
            }
        }
    }
}
