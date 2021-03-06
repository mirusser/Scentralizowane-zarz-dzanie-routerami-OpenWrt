﻿namespace RouterManagement.Models.ViewModels.Router
{
    public class RouterActivityViewModel
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string RouterIp { get; set; }
        public int? Port { get; set; }
        public string Login { get; set; }
    }
}