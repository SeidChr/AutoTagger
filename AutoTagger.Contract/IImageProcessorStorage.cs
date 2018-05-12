namespace AutoTagger.Contract
{
    using System.Collections.Generic;

    public interface IImageProcessorStorage
    {
        void DoSave();

        IEnumerable<IImage> GetImagesWithoutMachineTags(int limit);

        IEnumerable<IImage> GetImagesWithoutMachineTags(int idLargerThan, int limit);

        void InsertMachineTagsWithoutSaving(IImage image);
    }
}
