using Entities.Models;
using Shared.RequestFeatures;

namespace Contacts.Interfaces.ModelRepository
{
    public interface ICompanyRepository
    {
        #region Get All
        Task<PagedList<Company>> GetAllCompaniesAsync(CompanyParameters companyParameters, bool trackChanges);
        
        Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
        #endregion

        #region Get By
        Task<Company> GetCompanyAsync(Guid id, bool trackChanges);
        #endregion

        // We don't add Task to Create and Delete function
        // because we don't do any async code in these function
        // we only set state of entity to Added or Deleted
        #region Create
        void CreateCompany(Company company);
        #endregion

        #region Update
        
        #endregion

        #region Delete
        void DeleteCompany(Company company);
        #endregion
    }
}
