namespace AutoTagger.Database.Standard.Repository
{
    using AutoTagger.Contract;

    public class CrawlerRepository : BaseRepository, ICrawlerRepository
    {
        private readonly ICrawlerContext context;

        public CrawlerRepository(ICrawlerContext context)
            : base(context)
        {
            this.context = context;
        }

        public void InsertOrUpdate(ICrawlerImage crawlerImage)
        {
            this.context.InsertOrUpdate(crawlerImage);
        }
    }
}
