using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface ICompanyService
    {
        IEnumerable<CompanyDTO> GetAllCompanies(bool trackChanges);

        CompanyDTO GetCompany(Guid id, bool trackChanges);

        CompanyDTO CreateCompany(CreateCompanyDTO createCompanyDTO);
    }
}
