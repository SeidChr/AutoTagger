namespace AutoTagger.Database.Storage.AutoTagger
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using global::AutoTagger.Contract;

    using Microsoft.EntityFrameworkCore;

    public class MysqlUIStorage : MysqlBaseStorage, IAutoTaggerStorage
    {
        public IEnumerable<string> FindHumanoidTags(IEnumerable<string> machineTags)
        {
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
                        yield return reader.GetValue(0).ToString();
                    }
                }
                this.db.Database.CloseConnection();
            }
        }

        private string BuildQuery(IEnumerable<string> machineTags)
        {
            var countInsertTags   = 3;
            var limitTopPhotos    = 200;
            var countTagsToReturn = 30;
            var whereCondition = BuildWhereCondition(machineTags);

            string query = $"SELECT i.name "
                         + $"FROM itags as i LEFT JOIN photo_itag_rel as rel ON rel.itagId = i.id LEFT JOIN "
                         + $"( SELECT p.id, ((count(m.name)-2 * matches + {countInsertTags}) / (count(m.name) "
                         + $"+ {countInsertTags} - matches)) *popularity as relationQuality "
                         + $"FROM photos as p LEFT JOIN mtags as m ON m.photoId = p.id LEFT JOIN "
                         + $"( SELECT p.id, (p.likes+p.comments)/ p.follower as popularity, count(m.name) as matches "
                         + $"FROM photos as p LEFT JOIN mtags as m ON m.photoId = p.id WHERE {whereCondition} "
                         + $"GROUP by p.id ) as sub1 ON p.id = sub1.id WHERE sub1.id IS NOT NULL "
                         + $"GROUP by p.id ORDER by relationQuality DESC LIMIT {limitTopPhotos} ) as sub2 ON sub2.id = rel.photoId "
                         + $"WHERE sub2.id IS NOT NULL GROUP by i.name ORDER by count(i.name) DESC, relationQuality DESC LIMIT {countTagsToReturn}";

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
    }
}
