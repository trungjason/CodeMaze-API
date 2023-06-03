namespace Entities.Exceptions.BaseException
{
    public abstract class BadRequestException : Exception 
    {
        public BadRequestException(string message) : base(message) 
        {
            
        }
    }
}
