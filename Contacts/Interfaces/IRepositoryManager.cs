using Contacts.Interfaces.ModelRepository;

namespace Contacts.Interfaces
{
    public interface IRepositoryManager
    {
        IEmployeeRepository Employee { get; }

        ICompanyRepository Company { get; }

        void Save();
    }
}
