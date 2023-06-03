using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface IEmployeeService
    {
        IEnumerable<EmployeeDTO> GetAllEmployees(Guid companyId, bool trackChanges);

        EmployeeDTO GetEmployee(Guid companyId, Guid id, bool trackChanges);

        EmployeeDTO CreateEmployee(Guid companyId, CreateEmployeeDTO createEmployeeDTO, bool trackChanges);
    }
}
