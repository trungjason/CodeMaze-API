using Contacts.Interfaces.ModelRepository;
using Entities.Models;

namespace Repository.ModelRepository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Company> GetAllCompanies(bool trackChanges)
        {
            return FindAll(trackChanges).OrderBy(company => company.Name).ToList();
        }
    }
}
