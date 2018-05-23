namespace AutoTagger.Storage.EntityFramework.Core
{
    using System;

    using Microsoft.EntityFrameworkCore;

    public abstract class EntityFrameworkBaseStorage
    {
        protected EntityFrameworkBaseStorage()
        {
            this.Db = new AutoTaggerEntityFrameworkContext();
        }

        protected AutoTaggerEntityFrameworkContext Db
        {
            get;
            set;
        }

        protected void Save()
        {
            try
            {
                this.Db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException.Message.Contains("Timeout"))
                {
                    this.Reconnect();
                    this.Db.SaveChanges();
                }
                else
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private void Reconnect()
        {
            this.Db.Database.CloseConnection();
            this.Db.Database.OpenConnection();
        }
    }
}
