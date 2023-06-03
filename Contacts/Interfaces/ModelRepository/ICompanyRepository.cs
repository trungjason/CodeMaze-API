using Entities.Models;

namespace Contacts.Interfaces.ModelRepository
{
    public interface ICompanyRepository
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

        IEnumerable<Company> GetAllCompanies(bool trackChanges);

        IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges);

        Company GetCompany(Guid id, bool trackChanges);

        void CreateCompany(Company company);
    }
}
