using Entities.LinkModels;
using Microsoft.AspNetCore.Http;
using Shared.DataTransferObjects;

namespace Contacts.Interfaces
{
    public interface IEmployeeLinks
    {
        LinkResponse TryGenerateLinks(
            IEnumerable<EmployeeDTO> employeesDto,
            string fields, 
            Guid companyId, 
            HttpContext httpContext
        );
    }
}
