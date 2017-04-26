using System.ComponentModel.DataAnnotations;

namespace RouterManagement.Logic.DataAnnotations
{
    public class StringNotNullNotEmptyAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            return !string.IsNullOrEmpty(value as string);
        }
    }
}
