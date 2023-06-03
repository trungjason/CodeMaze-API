namespace Entities.Exceptions
{
    public sealed class CompanyNotFoundException : NotFoundException
    {
        public CompanyNotFoundException(Guid id) : base($"The company with id: {id} could not be found.")
        {
            
        }
    }
}
