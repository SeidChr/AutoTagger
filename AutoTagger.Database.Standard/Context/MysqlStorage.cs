using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Database.Standard.Context
{
    using MySql.Data.MySqlClient;

    public class MysqlStorage : IDisposable
    {
        private const string IP = "78.46.178.185";
        private const string DB = "instatagger";
        private const string USER = "InstaTagger";
        private const string PW = "ovI5Aq3J0xOjjwXn";

        protected readonly MySqlConnection connection;

        public MysqlStorage()
        {
            var myConnectionString = $"SERVER={IP};" +
                                     $"DATABASE={DB};" +
                                     $"UID={USER};" +
                                     $"PASSWORD={PW};";
            connection = new MySqlConnection(myConnectionString);
            connection.Open();
        }

        public void Dispose()
        {
            connection.Close();
        }
    }
}
