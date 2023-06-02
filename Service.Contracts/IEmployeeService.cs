using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface IEmployeeService
    {
        IEnumerable<EmployeeDTO> GetAllEmployees(bool trackChanges);
    }
}
