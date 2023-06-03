using Entities.Models;

namespace Contacts.Interfaces.ModelRepository
{
    public interface IEmployeeRepository
    {
        #region Constructor
        #endregion

        #region Get All
        #endregion

        #region Get By
        #endregion

        #region Create
        #endregion

        #region Update
        #endregion

        #region Delete
        #endregion

        IEnumerable<Employee> GetAllEmployees(Guid companyId, bool trackChanges);

        Employee GetEmployee(Guid companyId, Guid id, bool trackChanges);

        void CreateEmployeeForCompany(Guid companyId, Employee employee);
    }
}
