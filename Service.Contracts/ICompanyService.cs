using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface ICompanyService
    {
        #region Get All
        IEnumerable<CompanyDTO> GetAllCompanies(bool trackChanges);

        IEnumerable<CompanyDTO> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
        #endregion

        #region Get By
        CompanyDTO GetCompany(Guid id, bool trackChanges);
        #endregion

        #region Create
        CompanyDTO CreateCompany(CreateCompanyDTO createCompanyDTO);

        (IEnumerable<CompanyDTO> companies, string ids) CreateCompanyCollection(IEnumerable<CreateCompanyDTO> companyCollection);
        #endregion

        #region Update
        #endregion

        #region Delete
        #endregion
    }
}
