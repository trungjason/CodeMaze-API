using Contacts.Interfaces.ModelRepository;
using Entities.Models;

namespace Repository.ModelRepository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
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

        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public IEnumerable<Employee> GetAllEmployees(Guid companyId, bool trackChanges)
        {
            return FindByCondition(employee => employee.CompanyId.Equals(companyId) ,trackChanges)
                .OrderBy(employee => employee.Name).ToList();
        }

        public Employee GetEmployee(Guid companyId, Guid id, bool trackChanges)
        {
            return FindByCondition(employee => employee.Id.Equals(id) && employee.CompanyId.Equals(companyId), trackChanges)
                .SingleOrDefault();
        }
    }
}
