using AutoMapper;
using Contacts.Interfaces;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service
{
    internal sealed class EmployeeService : IEmployeeService
    {
        #region Constructor
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
        #endregion

        #region Get All
        public async Task<IEnumerable<EmployeeDTO>> GetEmployeesAsync(
            Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
            await _CheckIfCompanyExistsAsync(companyId, trackChanges);

            var employees = await _repository.Employee.GetEmployeesAsync(companyId, employeeParameters, trackChanges);

            var employeesDTO = _mapper.Map<IEnumerable<EmployeeDTO>>(employees);

            return employeesDTO;
        }
        #endregion

        #region Get By
        public async Task<EmployeeDTO> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await _CheckIfCompanyExistsAsync(companyId, trackChanges);

            var employee = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);

            var employeeDTO = _mapper.Map<EmployeeDTO>(employee);

            return employeeDTO;
        }

        public async Task<(UpdateEmployeeDTO employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(
            Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
        {
            await _CheckIfCompanyExistsAsync(companyId, compTrackChanges);

            var employeeDb = await _GetEmployeeForCompanyAndCheckIfItExistsAsync(companyId, id, empTrackChanges);

            var employeeToPatch = _mapper.Map<UpdateEmployeeDTO>(employeeDb);

            return (employeeToPatch: employeeToPatch, employeeEntity: employeeDb);
        }
        #endregion

        #region Create
        public async Task<EmployeeDTO> CreateEmployeeForCompanyAsync(Guid companyId, CreateEmployeeDTO createEmployeeDTO, bool trackChanges)
        {
            await _CheckIfCompanyExistsAsync(companyId, trackChanges);

            var employeeEntity = _mapper.Map<Employee>(createEmployeeDTO);

            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            await _repository.SaveAsync();

            var employeeToReturn = _mapper.Map<EmployeeDTO>(employeeEntity);

            return employeeToReturn;
        }
        #endregion

        #region Update
        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, UpdateEmployeeDTO updateEmployeeDTO, bool companyTrackChanges, bool employeeTrackChanges)
        {
            await _CheckIfCompanyExistsAsync(companyId, companyTrackChanges);

            var employeeEntity = await _GetEmployeeForCompanyAndCheckIfItExistsAsync(companyId, id, employeeTrackChanges);

            _mapper.Map(updateEmployeeDTO, employeeEntity);

            await _repository.SaveAsync();
        }

        public async Task SaveChangesForPatchAsync(UpdateEmployeeDTO employeeToPatch, Employee employeeEntity)
        {
            _mapper.Map(employeeToPatch, employeeEntity);
            await _repository.SaveAsync();
        }
        #endregion

        #region Delete
        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await _CheckIfCompanyExistsAsync(companyId, trackChanges);

            var employeeEntity = await _GetEmployeeForCompanyAndCheckIfItExistsAsync(companyId, id, trackChanges);

            _repository.Employee.DeleteEmployee(employeeEntity);
            await _repository.SaveAsync();
        }
        #endregion

        #region Methods
        private async Task _CheckIfCompanyExistsAsync(Guid companyId, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);

            if (company is null)
                throw new CompanyNotFoundException(companyId);
        }

        private async Task<Employee> _GetEmployeeForCompanyAndCheckIfItExistsAsync(Guid companyId, Guid id, bool trackChanges)
        {
            var employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);

            if (employeeDb is null)
                throw new EmployeeNotFoundException(id);

            return employeeDb;
        }
        #endregion
    }
}