namespace RouterManagement.Models.ViewModels
{
    public class AddFirewallRuleViewModel
    {
        public string Type { get; set; }
        public bool Is_Ingreee { get; set; }
        public string Description { get; set; }
        public string Local_addr { get; set; }
        public string Active_hours { get; set; }
        public bool Enabled { get; set; }
    }
}