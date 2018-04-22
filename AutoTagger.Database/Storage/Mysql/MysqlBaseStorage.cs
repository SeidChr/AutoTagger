namespace AutoTagger.Database.Storage
{
    using System;
    using global::AutoTagger.Database.Mysql;
    using Microsoft.EntityFrameworkCore;
    using MySql.Data.MySqlClient;

    public abstract class MysqlBaseStorage
    {
        protected InstataggerContext db;

        protected MysqlBaseStorage()
        {
            db = new InstataggerContext();
        }

        private void Reconnect()
        {
            this.db.Database.CloseConnection();
            this.db.Database.OpenConnection();
        }

        protected bool Save(Action func)
        {
            try
            {
                this.db.SaveChanges();
                return true;
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Timeout"))
                {
                    this.Reconnect();
                    func();
                }
                else
                {
                    Console.WriteLine(e);
                    throw;
                }
                return false;
            }
        }
    }

}
