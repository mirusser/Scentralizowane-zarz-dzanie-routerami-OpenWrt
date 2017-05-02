using System.Collections.Generic;

namespace RouterManagement.Models.ViewModels
{
    public class FirewallViewModel
    {
        public List<FirewallRuleViewModel> FirewallRestrictionRules { get; set; }
        public string RouterName { get; set; }
    }
}