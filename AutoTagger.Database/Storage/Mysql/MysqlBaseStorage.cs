namespace AutoTagger.Database.Storage
{
    using global::AutoTagger.Database.Mysql;
    using Microsoft.EntityFrameworkCore;

    public abstract class MysqlBaseStorage
    {
        protected InstataggerContext db;

        protected MysqlBaseStorage()
        {
            db = new InstataggerContext();
        }

        public void Reconnect()
        {
            this.db.Database.CloseConnection();
            this.db.Database.OpenConnection();
        }
    }

}
