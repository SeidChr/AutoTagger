using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Database.Standard
{
    using AutoTagger.Contract;

    public class BaseRepository : IDisposable
    {
        protected IContext _context;

        protected bool isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed && disposing)
            {
                _context.Dispose();
            }
            this.isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
