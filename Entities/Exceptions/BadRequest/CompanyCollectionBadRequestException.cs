using Entities.Exceptions.BaseException;

namespace Entities.Exceptions.BadRequest
{
    public class CompanyCollectionBadRequestException : BadRequestException
    {
        public CompanyCollectionBadRequestException() : base("Company collection is empty.")
        {
            
        }

    }
}
