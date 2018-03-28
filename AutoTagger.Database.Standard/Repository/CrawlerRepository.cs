using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Database.Standard
{
    using AutoTagger.Contract;

    public class CrawlerRepository : BaseRepository, ICrawlerRepository
    {
        public CrawlerRepository(ICrawlerContext context)
        {
            _context = context;
        }

        public void InsertOrUpdate(ICrawlerImage crawlerImage)
        {
            (_context as ICrawlerContext).InsertOrUpdate(crawlerImage);
        }
    }
}
