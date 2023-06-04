using Contacts.Interfaces.ModelRepository;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace Repository.ModelRepository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        #region Constructor
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        #endregion

        #region Get All
        public async Task<IEnumerable<Employee>> GetEmployeesAsync(
            Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
            return await FindByCondition(employee => employee.CompanyId.Equals(companyId), trackChanges)
                .OrderBy(employee => employee.Name)
                .Skip((employeeParameters.PageNumber - 1 ) * employeeParameters.PageSize)
                .Take(employeeParameters.PageSize)
                .ToListAsync();
        }
        #endregion

        #region Get By
        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
            return await FindByCondition(employee => employee.Id.Equals(id) && employee.CompanyId.Equals(companyId), trackChanges)
                .SingleOrDefaultAsync();
        }
        #endregion

        #region Create
        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }
        #endregion

        #region Update
        #endregion

        #region Delete
        public void DeleteEmployee(Employee employee)
        {
            Delete(employee);
        }
        #endregion
    }
}
