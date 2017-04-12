using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouterManagement.Models.ViewModels
{
    public class FirewallViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public bool Is_Ingreee { get; set; }
        public string Description { get; set; }
        public ICollection<string> Local_addr { get; set; }
        public ICollection<string> Active_hours { get; set; }
        public bool Enabled { get; set; }
    }
}