using Contacts.Interfaces.ModelRepository;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
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
        public async Task<PagedList<Employee>> GetEmployeesAsync(
            Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
            var employees = await FindByCondition((employee) => employee.CompanyId.Equals(companyId), trackChanges)
                .FilterEmployeesByAgeRange(employeeParameters.MinAge, employeeParameters.MaxAge)
                .SearchEmployeeByName(employeeParameters.SearchTerm)
                .Sort(employeeParameters.OrderBy)
                .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
                .Take(employeeParameters.PageSize)
                .ToListAsync();

            // We Pagination first for better performance and in case we have million of data row
            // then we will calculate count later for adding to metadata
            var count = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges).CountAsync();

            return PagedList<Employee>.ToPagedList(employees, count, employeeParameters.PageNumber, employeeParameters.PageSize);
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
