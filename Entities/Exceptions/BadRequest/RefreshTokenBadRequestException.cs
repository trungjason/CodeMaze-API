using Entities.Exceptions.BaseException;

namespace Entities.Exceptions.BadRequest
{
    public class RefreshTokenBadRequestException : BadRequestException
    {
        public RefreshTokenBadRequestException() : base("Refrest Token Bad Request.")
        {
            
        }
    }
}
