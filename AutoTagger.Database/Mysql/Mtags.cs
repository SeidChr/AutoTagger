namespace AutoTagger.Database.Mysql
{
    public class Mtags
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Photos Photo { get; set; }

        public int PhotoId { get; set; }

        public float Score { get; set; }

        public string Source { get; set; }
    }
}
