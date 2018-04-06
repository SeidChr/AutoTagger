using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Database.Standard.Context.AutoTagger
{
    using global::AutoTagger.Contract;
    using MySql.Data.MySqlClient;

    public class MysqlAutoTaggerStorage : IAutoTaggerStorage, IDisposable
    {
        private const string IP = "78.46.178.185";
        private const string DB = "instatagger";
        private const string USER = "InstaTagger";
        private const string PW = "ovI5Aq3J0xOjjwXn";

        private readonly MySqlConnection connection;

        public MysqlAutoTaggerStorage()
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

        public IEnumerable<string> FindHumanoidTags(IEnumerable<string> machineTags)
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = BuildQuery(machineTags);

            var output = new List<string>();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var row = "";
                for (var i = 0; i < reader.FieldCount; i++)
                    row += reader.GetValue(i) + ", ";
                output.Add(row);
            }
            return output;
        }

        public void Remove(string imageId)
        {
        }

        public void InsertOrUpdate(ICrawlerImage crawlerImage)
        {
            MySqlCommand comm = this.connection.CreateCommand();
            //comm.CommandText  = "INSERT INTO photos(person,address) VALUES(@person, @address)";
            //comm.Parameters.Add("@person", "Myname");
            //comm.Parameters.Add("@address", "Myaddress");
            comm.ExecuteNonQuery();
        }

        public void InsertOrUpdate(string imageId, IEnumerable<string> machineTags, IEnumerable<string> humanoidTags)
        {
            throw new NotImplementedException();
        }

        private string BuildQuery(IEnumerable<string> machineTags)
        {
            var countInsertTags = 3;
            var countTopPhotos = 10;
            var numberOfTagsIWantToGet = 30;

            var whereCondition = "";
            foreach (var machineTag in machineTags)
            {
                if (string.IsNullOrEmpty(machineTag))
                    continue;
                whereCondition += $"`m`.`value` = '{machineTag}' OR ";
            }

            char[] charsToTrim = { ' ', 'O', 'R' };
            whereCondition = whereCondition.Trim(charsToTrim);

            string query = "SELECT i.id, i.value, relationQuality, count(i.value) FROM itags as i LEFT JOIN ( SELECT p.id, "
                + $"(count(m.value) - 2 * matches + {countInsertTags}) / (count(m.value) + {countInsertTags} - matches) * popularity as relationQuality "
                + "FROM photos as p LEFT JOIN mtags as m ON m.photoId = p.id "
                + "LEFT JOIN ( SELECT p.id, (p.likes+p.comments)/p.follower as popularity, count(m.value) as matches "
                + "FROM photos as p LEFT JOIN mtags as m ON m.photoId = p.id "
                + $"WHERE {whereCondition} "
                + "GROUP by p.id ) as sub1 ON p.id = sub1.id WHERE sub1.id IS NOT NULL "
                + $"GROUP by p.id ORDER by relationQuality DESC LIMIT {countTopPhotos} ) as sub2 ON sub2.id = i.photoId "
                + "WHERE sub2.id IS NOT NULL "
                + "GROUP by i.value ORDER by count(i.value) DESC, relationQuality DESC "
                + $"LIMIT {numberOfTagsIWantToGet}"
                ;

            return query;
        }

    }
}
