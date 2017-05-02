using System.Collections.Generic;
using RouterManagement.Logic.DataAnnotations;

namespace RouterManagement.Models.ViewModels.Router
{
    public class AddRouterPartialViewModel
    {
        [NotNull]
        public IEnumerable<string> AllRoutersNames { get; set; }
    }
}