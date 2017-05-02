using System.ComponentModel.DataAnnotations;
using RouterManagement.Logic.DataAnnotations;

namespace RouterManagement.Models.ViewModels.Firewall
{
    public class AddFirewallRuleViewModel
    {
        public string RouterName { get; set; }
        public string FriendlyName { get; set; }
        public string Src_mac { get; set; }
        public string Src_ip { get; set; }
        public string Src_port { get; set; }
        public string Dest_ip { get; set; }
        public string Dest_port { get; set; }
        public string Enabled { get; set; }
    }
}