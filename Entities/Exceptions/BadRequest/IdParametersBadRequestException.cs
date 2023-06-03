using Entities.Exceptions.BaseException;

namespace Entities.Exceptions.BadRequest
{
    public sealed class IdParametersBadRequestException : BadRequestException
    {
        public IdParametersBadRequestException() : base("Parameter ids is null")
        {
            
        }
    }
}
