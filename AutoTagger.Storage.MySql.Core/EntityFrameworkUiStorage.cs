namespace AutoTagger.Storage.EntityFramework.Core
{
    using System.Collections.Generic;
    using System.Data;

    using AutoTagger.Contract;

    using Microsoft.EntityFrameworkCore;

    public class EntityFrameworkUiStorage : EntityFrameworkBaseStorage, IAutoTaggerStorage
    {
        public (string debug, IEnumerable<string> htags) FindHumanoidTags(List<IMachineTag> machineTags)
        {
            var htags = new List<string>();

            ////machineTags.RemoveAll(x => x.Name.StartsWith("no "));
            var query = this.BuildQuery(machineTags);

            using (var command = this.Db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                this.Db.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var htag = reader.GetValue(0).ToString();
                        htags.Add(htag);
                    }
                }

                this.Db.Database.CloseConnection();
            }

            return (query, htags);
        }

        public void Log(string source, string data)
        {
            var debug = new Debug { Source = source, Data = data };
            this.Db.Debug.Add(debug);
            this.Db.SaveChanges();
        }

        private static string BuildWhereCondition(IEnumerable<IMachineTag> machineTags, string source)
        {
            var whereCondition = string.Empty;
            foreach (var machineTag in machineTags)
            {
                if (machineTag.Source != source)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(machineTag.Name))
                {
                    continue;
                }

                whereCondition += $"`m`.`name` = '{machineTag.Name}' OR ";
            }

            char[] charsToTrim = { ' ', 'O', 'R' };
            whereCondition = whereCondition.Trim(charsToTrim);
            return whereCondition;
        }

        private string BuildQuery(IEnumerable<IMachineTag> machineTags)
        {
            var limitTopPhotos      = 100;
            var countTagsToReturn   = 30;
            var whereConditionLabel = BuildWhereCondition(machineTags, "GCPVision_Label");
            var whereConditionWeb   = BuildWhereCondition(machineTags, "GCPVision_Web");

            var query = $"SELECT i.name " + $"FROM itags as i LEFT JOIN photo_itag_rel as rel ON rel.itagId = i.id "
                      + $"LEFT JOIN ( SELECT p.id, count(m.name) as matches FROM photos as p "
                      + $"LEFT JOIN mtags as m ON m.photoId = p.id " + $"WHERE "
                      + $"(({whereConditionLabel}) AND m.source='GCPVision_Label')"
                      + $"OR (({whereConditionWeb}) AND m.source='GCPVision_Web')"
                      + $" GROUP BY p.id ORDER BY matches DESC LIMIT {limitTopPhotos} "
                      + $") as sub2 ON sub2.id = rel.photoId WHERE sub2.id IS NOT NULL "
                      + $"GROUP BY i.name ORDER by sum(matches) DESC LIMIT {countTagsToReturn}";

            return query;
        }
    }
}
