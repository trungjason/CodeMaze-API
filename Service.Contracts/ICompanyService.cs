using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service.Contracts
{
    public interface ICompanyService
    {
        #region Get All
        Task<(IEnumerable<CompanyDTO> companies, MetaData metaData)> GetAllCompaniesAsync(CompanyParameters companyParameters, bool trackChanges);
    
        Task<IEnumerable<CompanyDTO>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
        #endregion

        #region Get By
        Task<CompanyDTO> GetCompanyAsync(Guid id, bool trackChanges);
        #endregion

        #region Create
        Task<CompanyDTO> CreateCompanyAsync(CreateCompanyDTO createCompanyDTO);

        Task<(IEnumerable<CompanyDTO> companies, string ids)> CreateCompanyCollectionAsync(IEnumerable<CreateCompanyDTO> companyCollection);
        #endregion

        #region Update
        Task UpdateCompanyAsync(Guid companyId, UpdateCompanyDTO updateCompanyDTO, bool trackChanges);
        #endregion

        #region Delete
        Task DeleteCompanyAsync(Guid companyId, bool trackChanges);
        #endregion
    }
}
