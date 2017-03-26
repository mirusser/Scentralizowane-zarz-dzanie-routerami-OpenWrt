using System.ComponentModel.DataAnnotations;

namespace RouterManagement.Models
{
    public class CommonSetting
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string Setting { get; set; }
        public string Value { get; set; }
    }
}