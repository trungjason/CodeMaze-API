using Entities.Models;

namespace Contacts.Interfaces.ModelRepository
{
    public interface ICompanyRepository
    {
        IEnumerable<Company> GetAllCompanies(bool trackChanges);

        Company GetCompany(Guid id, bool trackChanges);
    }
}
