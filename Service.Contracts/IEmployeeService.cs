using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface IEmployeeService
    {
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

        IEnumerable<EmployeeDTO> GetAllEmployees(Guid companyId, bool trackChanges);

        EmployeeDTO GetEmployee(Guid companyId, Guid id, bool trackChanges);

        EmployeeDTO CreateEmployeeForCompany(Guid companyId, CreateEmployeeDTO createEmployeeDTO, bool trackChanges);
    }
}
