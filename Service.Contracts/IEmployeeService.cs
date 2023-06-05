using Entities.LinkModels;
using Entities.Models;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Dynamic;

namespace Service.Contracts
{
    public interface IEmployeeService
    {
        #region Get All
        Task<(LinkResponse linkResponse, MetaData metaData)> GetEmployeesAsync(Guid companyId, LinkParameters linkParameters, bool trackChanges);
        #endregion

        #region Get By
        Task<EmployeeDTO> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges);

        Task<(UpdateEmployeeDTO employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(
            Guid companyId,
            Guid id,
            bool compTrackChanges,
            bool empTrackChanges
        );
        #endregion

        #region Create
        Task<EmployeeDTO> CreateEmployeeForCompanyAsync(Guid companyId, CreateEmployeeDTO createEmployeeDTO, bool trackChanges);
        #endregion

        #region Update
        Task UpdateEmployeeForCompanyAsync(
            Guid companyId,
            Guid id,
            UpdateEmployeeDTO updateEmployeeDTO, 
            bool companyTrackChanges,
            bool employeeTrackChanges
        );

        Task SaveChangesForPatchAsync(
            UpdateEmployeeDTO employeeToPatch, 
            Employee employeeEntity
        );
        #endregion

        #region Delete
        Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges);
        #endregion
    }
}
