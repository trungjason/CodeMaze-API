using Contacts.Interfaces.ModelRepository;
using Entities.Models;

namespace Repository.ModelRepository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
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

        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Company> GetAllCompanies(bool trackChanges)
        {
            return FindAll(trackChanges).OrderBy(company => company.Name).ToList();
        }

        public Company GetCompany(Guid id, bool trackChanges)
        {
            return FindByCondition(company => company.Id.Equals(id), trackChanges).SingleOrDefault();
        }

        public void CreateCompany(Company company)
        {
            Create(company);
        }

        public IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
        {
            return FindByCondition(company => ids.Contains(company.Id), trackChanges)
                .OrderBy(company => company.Name)
                .ToList();
        }
    }
}
