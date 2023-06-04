using Entities.Models;
using Shared.RequestFeatures;

namespace Contacts.Interfaces.ModelRepository
{
    public interface IEmployeeRepository
    {
        #region Get All
        Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges);
        #endregion

        #region Get By
        Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges);
        #endregion

        #region Create
        void CreateEmployeeForCompany(Guid companyId, Employee employee);
        #endregion

        #region Update
        #endregion

        #region Delete
        void DeleteEmployee(Employee employee);
        #endregion
    }
}
