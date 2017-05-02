using RouterManagement.Logic.DataAnnotations;

namespace RouterManagement.Models.ViewModels.Firewall
{
    public class FirewallRulePartialViewModel
    {
        [StringNotNullNotEmpty]
        public string RuleName { get; set; }

        [StringNotNullNotEmpty]
        public string RouterName { get; set; }
    }
}