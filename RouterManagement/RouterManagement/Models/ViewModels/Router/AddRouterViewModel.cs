using System.ComponentModel.DataAnnotations;
using RouterManagement.Logic.DataAnnotations;

namespace RouterManagement.Models.ViewModels.Router
{
    public class AddRouterViewModel
    {
        [StringNotNullNotEmpty]
        public string Name { get; set; }
        [RegularExpression(@"^((((([a-zA-Z]+|[a-zA-Z0-9]*)\.)+)(([a-zA-Z]+[a-zA-Z0-9]*)+))|((\d|[1-9]\d|1\d\d|2([0-4]\d|5[0-5]))\.(\d|[1-9]\d|1\d\d|2([0-4]\d|5[0-5]))\.(\d|[1-9]\d|1\d\d|2([0-4]\d|5[0-5]))\.(\d|[1-9]\d|1\d\d|2([0-4]\d|5[0-5])))|(localhost))$")]
        public string RouterIp { get; set; }
        public int? Port { get; set; }
        [StringNotNullNotEmpty]
        public string Login { get; set; }
        public string Password { get; set; }
    }
}