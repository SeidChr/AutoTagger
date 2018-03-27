namespace AutoTagger.UserInterface.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ScanLinkModel
    {
        [Required]
        public string Link { get; set; }
    }
}
