namespace AutoTagger.Database.Mysql
{
    using System;

    public class Debug
    {
        public DateTimeOffset Created { get; set; }

        public string Data { get; set; }

        public int Id { get; set; }

        public string Source { get; set; }
    }
}
