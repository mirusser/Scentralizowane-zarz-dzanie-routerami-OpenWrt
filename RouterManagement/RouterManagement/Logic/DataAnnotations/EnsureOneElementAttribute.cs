using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace RouterManagement.Logic.DataAnnotations
{
    public class EnsureOneElementAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var list = value as IList;
            return list?.Count > 0;
        }
    }
}
