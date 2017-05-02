using System.Collections.Generic;
using RouterManagement.Logic.DataAnnotations;

namespace RouterManagement.Models.ViewModels.Firewall
{
    public class FirewallRuleViewModel
    {
        [StringNotNullNotEmpty]
        public string RuleName { get; set; }

        [StringNotNullNotEmpty]
        public string FriendlyName { get; set; }

        public ICollection<string> Src_mac { get; set; }

        public ICollection<string> Src_ip { get; set; }

        public ICollection<string> Src_port { get; set; }

        public ICollection<string> Dest_ip { get; set; }

        public ICollection<string> Dest_port { get; set; }

        [StringNotNullNotEmpty]
        public string Enabled { get; set; }
    }
}