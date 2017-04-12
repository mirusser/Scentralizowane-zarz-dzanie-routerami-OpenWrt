using System.ComponentModel.DataAnnotations;

namespace RouterManagement.Models.ViewModels
{
    public class SendUciShowWirelessViewModel
    {
        public bool Disabled { get; set; }
        [Range(0, 13)]//TODO check it
        public int Channel { get; set; }
        public string Ssid { get; set; }
        public string Encryption = "psk2";
        public string Key { get; set; }
        public string Mode { get; set; } //sta or ap
        public string Network { get; set; } //lan or wan
    }
}