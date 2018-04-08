namespace AutoTagger.Database.Context.Crawler
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    using global::AutoTagger.Contract;
    using global::AutoTagger.Database.Context;

    using MySql.Data.MySqlClient;

    public class MysqlCrawlerStorage : MysqlStorage, ICrawlerStorage
    {
        private MySqlCommand command;

        public void InsertOrUpdate(IImage image)
        {
            //this.command = this.connection.CreateCommand();
            //var lastOldId = this.command.LastInsertedId;

            //InsertPhoto(image);
            //InsertHumanoidTags(image.ImageId, image.HumanoidTags);
        }

        //private void InsertPhoto(IImage image)
        //{
        //    this.command.CommandText = "INSERT INTO photos(img,likes,comments,follower) VALUES(@img, @likes,@comments,@follower)";
        //    this.command.Parameters.AddWithValue("@img", image.ImageUrl);
        //    this.command.Parameters.AddWithValue("@likes", image.Likes);
        //    this.command.Parameters.AddWithValue("@comments", image.Comments);
        //    this.command.Parameters.AddWithValue("@follower", image.Follower);
        //    this.command.ExecuteNonQuery();
        //    var lastNewId = this.command.LastInsertedId;
        //    image.ImageId = lastNewId.ToString();
        //}

        private void InsertHumanoidTags(string imageId, IEnumerable<string> humonoidTags)
        {
        }

        private void InsertMachineTags(IEnumerable<string> machineTags)
        {
        }
    }
}
