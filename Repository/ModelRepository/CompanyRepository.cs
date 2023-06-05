using Contacts.Interfaces.ModelRepository;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace Repository.ModelRepository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        #region Constructor
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        #endregion

        #region Get All
        public async Task<PagedList<Company>> GetAllCompaniesAsync(CompanyParameters companyParameters, bool trackChanges)
        {
            var companies = await FindAll(trackChanges)
                .OrderBy(company => company.Name)
                .Skip((companyParameters.PageSize - 1) * companyParameters.PageNumber)
                .Take(companyParameters.PageSize)
                .ToListAsync();

            var count = await FindAll(trackChanges).CountAsync();


            return PagedList<Company>.ToPagedList(companies, count, pageNumber: companyParameters.PageNumber, pageSize: companyParameters.PageSize);
        }

        public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
        {
            return await FindByCondition(company => ids.Contains(company.Id), trackChanges)
                .OrderBy(company => company.Name)
                .ToListAsync();
        }
        #endregion

        #region Get By
        public async Task<Company> GetCompanyAsync(Guid id, bool trackChanges)
        {
            return await FindByCondition(company => company.Id.Equals(id), trackChanges)
                    .SingleOrDefaultAsync();
        }
        #endregion

        #region Create
        public void CreateCompany(Company company)
        {
            Create(company);
        }
        #endregion

        #region Update
        
        #endregion

        #region Delete
        public void DeleteCompany(Company company)
        {
            Delete(company);
        }
        #endregion
    }
}
