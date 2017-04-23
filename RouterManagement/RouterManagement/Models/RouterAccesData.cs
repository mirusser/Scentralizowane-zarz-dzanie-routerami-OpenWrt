using System.ComponentModel.DataAnnotations;
using System.Net;

namespace RouterManagement.Models
{
    public class RouterAccesData
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public string RouterIp { get; set; }
        public int? Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}