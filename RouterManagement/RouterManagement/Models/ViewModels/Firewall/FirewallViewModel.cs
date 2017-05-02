using System.Collections.Generic;
using RouterManagement.Logic.DataAnnotations;

namespace RouterManagement.Models.ViewModels.Firewall
{
    public class FirewallViewModel
    {
        public List<FirewallRuleViewModel> FirewallRestrictionRules { get; set; }

        [StringNotNullNotEmpty]
        public string RouterName { get; set; }
    }
}