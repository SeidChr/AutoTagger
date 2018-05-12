namespace AutoTagger.Database.Mysql
{
    public class PhotoItagRel
    {
        public int Id { get; set; }

        public Itags Itag { get; set; }

        public int ItagId { get; set; }

        public Photos Photo { get; set; }

        public int PhotoId { get; set; }
    }
}
