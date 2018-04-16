namespace AutoTagger.Database.Context
{
    using global::AutoTagger.Database.Mysql;
    using Microsoft.EntityFrameworkCore;

    public abstract class MysqlStorage
    {
        protected InstataggerContext db;

        protected MysqlStorage()
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
