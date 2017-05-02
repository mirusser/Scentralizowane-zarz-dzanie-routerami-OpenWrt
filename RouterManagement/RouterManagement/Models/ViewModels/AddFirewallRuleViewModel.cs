using System.ComponentModel.DataAnnotations;
using RouterManagement.Logic.DataAnnotations;

namespace RouterManagement.Models.ViewModels
{
    public class AddFirewallRuleViewModel
    {
        public string RuleName { get; set; }
        public string FriendlyName { get; set; }
        public string Src_mac { get; set; }
        public string Src_ip { get; set; }
        public string Src_port { get; set; }
        public string Dest_ip { get; set; }
        public string Dest_port { get; set; }
        public string Enabled { get; set; }
    }
}