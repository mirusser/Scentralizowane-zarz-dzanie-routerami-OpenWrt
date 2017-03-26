using System.ComponentModel.DataAnnotations;

namespace RouterManagement.Models
{
    public class ConfigSetting
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}