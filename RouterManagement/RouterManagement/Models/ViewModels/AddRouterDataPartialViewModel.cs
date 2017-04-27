using System.Collections.Generic;
using RouterManagement.Logic.DataAnnotations;

namespace RouterManagement.Models.ViewModels
{
    public class AddRouterDataPartialViewModel
    {
        [NotNull]
        public IEnumerable<string> AllRoutersNames { get; set; }
    }
}