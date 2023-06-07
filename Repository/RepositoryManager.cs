using Contacts.Interfaces;
using Contacts.Interfaces.ModelRepository;
using Repository.ModelRepository;

namespace Repository
{
    // We are using this Manager to manage all Repository like Company or Employee
    // Because in our API, if we want to fetch multi resource like 1 company and some epmloyee
    // So we should encapsulate all repository into this manager
    // => all Controller can use
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly Lazy<ICompanyRepository> _companyRepository;
        private readonly Lazy<IEmployeeRepository> _employeeRepository;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;

            // Because we are using Lazy Initialize => We can't use DI in CompanyRepository
            _companyRepository = new Lazy<ICompanyRepository>(() => new CompanyRepository(repositoryContext));
            _employeeRepository = new Lazy<IEmployeeRepository>(() => new EmployeeRepository(repositoryContext));
        }

        public ICompanyRepository Company => _companyRepository.Value;

        public IEmployeeRepository Employee => _employeeRepository.Value;

        // In EntityFramework our action like Create, Delete, Update only
        // have affect when we use Save function.
        // We don't use Save function in Create or Delete, Update function
        // in RepositoryBase because we can have multi operation before Save
        // Like: Create(a), Delete(b), Update(c) then Save()
        public async Task SaveAsync()
        {
            await _repositoryContext.SaveChangesAsync();
        }
    }
}
