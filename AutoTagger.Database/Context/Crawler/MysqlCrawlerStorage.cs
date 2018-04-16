namespace AutoTagger.Database.Context.Crawler
{
    using System.Linq;
    using global::AutoTagger.Contract;
    using global::AutoTagger.Database.Context;
    using global::AutoTagger.Database.Mysql;
    using MySql.Data.MySqlClient;

    public class MysqlCrawlerStorage : MysqlStorage, ICrawlerStorage
    {
        private MySqlCommand command;

        public void InsertOrUpdate(IImage image)
        {
            this.RemoveIfExisting(image);

            var photo = Photos.FromImage(image);
            this.db.Photos.Add(photo);
            this.Save(image);
            image.Shortcode = photo.Id.ToString();
        }

        private void Save(IImage image)
        {
            try
            {
                this.db.SaveChanges();
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Timeout"))
                {
                    this.Reconnect();
                    this.InsertOrUpdate(image);
                }
            }
        }

        private void RemoveIfExisting(IImage image)
        {
            var existingPhoto = this.db.Photos.FirstOrDefault(x => x.Shortcode == image.Shortcode);
            if (existingPhoto != null)
            {
                // TODO check
                //this.db.Itags.RemoveRange(this.db.Itags.Where(x => x.PhotoId == existingPhoto.Id));
                this.db.Mtags.RemoveRange(this.db.Mtags.Where(x => x.PhotoId == existingPhoto.Id));
                this.db.Photos.Remove(existingPhoto);
                this.db.SaveChanges();
            }
        }
    }
}
