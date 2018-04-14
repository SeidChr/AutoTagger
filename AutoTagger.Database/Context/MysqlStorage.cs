using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Database.Context
{
    using global::AutoTagger.Database.Mysql;

    using MySql.Data.MySqlClient;

    public class MysqlStorage
    {
        protected InstataggerContext db;

        public MysqlStorage()
        {
            db = new InstataggerContext();
        }
    }

}
