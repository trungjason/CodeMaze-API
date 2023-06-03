using AutoMapper;
using Contacts.Interfaces;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    internal sealed class EmployeeService : IEmployeeService
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

        public EmployeeDTO CreateEmployeeForCompany(Guid companyId, CreateEmployeeDTO createEmployeeDTO, bool trackChanges)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges);

            if (company is null)
            {
                throw new CompanyNotFoundException(companyId);
            };

            var employeeEntity = _mapper.Map<Employee>(createEmployeeDTO);

            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            _repository.Save();

            var employeeToReturn = _mapper.Map<EmployeeDTO>(employeeEntity);

            return employeeToReturn;
        }

        public IEnumerable<EmployeeDTO> GetAllEmployees(Guid companyId, bool trackChanges)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges);

            if (company == null)
            {
                throw new CompanyNotFoundException(companyId);
            };

            var employees = _repository.Employee.GetAllEmployees(companyId, trackChanges);

            var employeesDTO = _mapper.Map<IEnumerable<EmployeeDTO>>(employees);

            return employeesDTO;
        }

        public EmployeeDTO GetEmployee(Guid companyId, Guid id, bool trackChanges)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges);

            if (company == null)
            {
                throw new CompanyNotFoundException(companyId);
            };

            var employee = _repository.Employee.GetEmployee(companyId, id, trackChanges);

            if (employee == null)
            {
                throw new EmployeeNotFoundException(id);
            };

            var employeeDTO = _mapper.Map<EmployeeDTO>(employee);

            return employeeDTO;
        }
    }
}