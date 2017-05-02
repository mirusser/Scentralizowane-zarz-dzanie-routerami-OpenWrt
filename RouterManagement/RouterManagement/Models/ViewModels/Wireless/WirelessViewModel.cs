using System.ComponentModel.DataAnnotations;
using RouterManagement.Logic.DataAnnotations;

namespace RouterManagement.Models.ViewModels.Wireless
{
    public class WirelessViewModel
    {
        public bool Disabled { get; set; }

        [RegularExpression(@"^((1[0-3])|[0-9]|(auto))$")]
        public string Channel { get; set; }

        [StringNotNullNotEmpty]
        public string Ssid { get; set; }

        [RegularExpression(@"^(wep)|(psk(2)?)$")]
        public string Encryption { get; set; } = "psk2";

        public string Key { get; set; }

        [RegularExpression(@"^(sta)|(ap)$")]
        public string Mode { get; set; }

        [RegularExpression(@"^(lan)|(wan)$")]
        public string Network { get; set; }

        [StringNotNullNotEmpty]
        public string RouterName { get; set; }
    }
}