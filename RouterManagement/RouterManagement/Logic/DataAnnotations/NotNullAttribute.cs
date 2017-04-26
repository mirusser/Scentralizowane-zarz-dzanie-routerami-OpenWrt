using System.ComponentModel.DataAnnotations;

namespace RouterManagement.Logic.DataAnnotations
{
    public class NotNullAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value != null;
        }
    }
}
