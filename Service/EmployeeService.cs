using AutoMapper;
using Contacts.Interfaces;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    internal sealed class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public EmployeeService(
            IRepositoryManager repository,
            ILoggerManager logger,
            IMapper mapper
            )
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public IEnumerable<EmployeeDTO> GetAllEmployees(bool trackChanges)
        {
            return null;
        }
    }
}