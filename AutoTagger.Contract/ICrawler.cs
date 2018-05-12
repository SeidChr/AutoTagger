namespace AutoTagger.Contract
{
    using System;
    using System.Collections.Generic;

    public interface ICrawler
    {
        event Action<IHumanoidTag> OnHashtagFound;

        IEnumerable<IImage> DoCrawling(int limit, params string[] customTags);
    }
}
