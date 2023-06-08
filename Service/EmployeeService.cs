using AutoMapper;
using Contacts.Interfaces;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Service.Contracts;
using Service.DataShaping;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Dynamic;

namespace Service
{
    internal sealed class EmployeeService : IEmployeeService
    {
        #region Constructor
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IEmployeeLinks _employeeLinks;

        public EmployeeService(
            IRepositoryManager repository,
            ILoggerManager logger,
            IMapper mapper,
            IEmployeeLinks employeeLinks
            )
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _employeeLinks = employeeLinks;
        }
        #endregion

        #region Get All
        public async Task<(LinkResponse linkResponse, MetaData metaData)> GetEmployeesAsync(
            Guid companyId, LinkParameters linkParameters, bool trackChanges)
        {
            if (!linkParameters.EmployeeParameters.IsValidAgeRange)
            {
                throw new EmployeeInvalidAgeRangeBadRequestException();
            };

            await _CheckIfCompanyExistsAsync(companyId, trackChanges);

            var employeesWithMetaData = await _repository
                .Employee
                .GetEmployeesAsync(companyId, linkParameters.EmployeeParameters, trackChanges);

            var employeesDTO = _mapper.Map<IEnumerable<EmployeeDTO>>(employeesWithMetaData);

            var links = _employeeLinks.TryGenerateLinks(
                employeesDTO, linkParameters.EmployeeParameters.Fields, companyId, linkParameters.Context);


            return (linkResponse: links, metaData: employeesWithMetaData.MetaData);
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