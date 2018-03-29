namespace AutoTagger.Database.Standard.Repository
{
    using System;

    public class BaseRepository : IDisposable
    {
        private readonly IDisposable disposableContext;

        private bool isDisposed;

        public BaseRepository(IDisposable disposableContext)
        {
            this.disposableContext = disposableContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed && disposing)
            {
                this.disposableContext.Dispose();
            }

            this.isDisposed = true;
        }
    }
}
