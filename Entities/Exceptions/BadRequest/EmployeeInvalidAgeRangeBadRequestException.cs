using Entities.Exceptions.BaseException;

namespace Entities.Exceptions
{
    public sealed class EmployeeInvalidAgeRangeBadRequestException : BadRequestException
    {
        public EmployeeInvalidAgeRangeBadRequestException() : base("Max age must greater than min age.")
        {
            
        }
    }
}
