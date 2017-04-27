using System.ComponentModel.DataAnnotations;
using RouterManagement.Logic.DataAnnotations;

namespace RouterManagement.Models.ViewModels
{
    public class ModifyRouterDataViewModel
    {
        [StringNotNullNotEmpty]
        public string Name { get; set; }
        [StringNotNullNotEmpty]
        public string OldName { get; set; }
        [RegularExpression(@"^([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])(\.([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9]))*$")]
        public string RouterIp { get; set; }
        public int? Port { get; set; }
        [StringNotNullNotEmpty]
        public string Login { get; set; }
        public string Password { get; set; }
    }
}