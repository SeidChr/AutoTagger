namespace AutoTagger.Database.Storage.AutoTagger
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using global::AutoTagger.Contract;
    using global::AutoTagger.Database.Mysql;

    using Microsoft.EntityFrameworkCore;

    public class MysqlUIStorage : MysqlBaseStorage, IAutoTaggerStorage
    {
        public (string debug, IEnumerable<string> htags) FindHumanoidTags(List<string> machineTags)
        {
            var htags = new List<string>();
            machineTags.RemoveAll(x => x.StartsWith("no "));
            var query = BuildQuery(machineTags);

            using (var command = this.db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                this.db.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var htag = reader.GetValue(0).ToString();
                        htags.Add(htag);
                    }
                }
                this.db.Database.CloseConnection();
            }

            return (query, htags);
        }

        private string BuildQuery(IEnumerable<string> machineTags)
        {
            var limitTopPhotos    = 100;
            var countTagsToReturn = 20;
            var whereCondition = BuildWhereCondition(machineTags);

            string query = $"SELECT i.name "
                         + $"FROM itags as i LEFT JOIN photo_itag_rel as rel ON rel.itagId = i.id "
                         + $"LEFT JOIN ( SELECT p.id, count(m.name) as matches FROM photos as p "
                         + $"LEFT JOIN mtags as m ON m.photoId = p.id "
                         + $"WHERE {whereCondition} GROUP BY p.id ORDER BY matches DESC LIMIT {limitTopPhotos} "
                         + $") as sub2 ON sub2.id = rel.photoId WHERE sub2.id IS NOT NULL "
                         + $"GROUP BY i.name ORDER by sum(matches) DESC LIMIT {countTagsToReturn}";

            return query;
        }

        private static string BuildWhereCondition(IEnumerable<string> machineTags)
        {
            var whereCondition = "";
            foreach (var machineTag in machineTags)
            {
                if (string.IsNullOrEmpty(machineTag))
                    continue;
                whereCondition += $"`m`.`name` = '{machineTag}' OR ";
            }

            char[] charsToTrim = { ' ', 'O', 'R' };
            whereCondition     = whereCondition.Trim(charsToTrim);
            return whereCondition;
        }

        public void Log(string source, string data)
        {
            var debug = new Debug { Source = source, Data = data};
            this.db.Debug.Add(debug);
            this.db.SaveChanges();
        }
    }
}
