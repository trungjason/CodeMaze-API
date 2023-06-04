using AutoMapper;
using Contacts.Interfaces;
using Entities.Exceptions;
using Entities.Exceptions.BadRequest;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    internal sealed class CompanyService : ICompanyService
    {
        #region Constructor
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompanyService(
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
        public async Task<IEnumerable<CompanyDTO>> GetAllCompaniesAsync(bool trackChanges)
        {
            var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges);

            var companiesDTO = _mapper.Map<IEnumerable<CompanyDTO>>(companies);

            return companiesDTO;
        }
        #endregion

        #region Get By
        public async Task<CompanyDTO> GetCompanyAsync(Guid companyId, bool trackChanges)
        {
            var company = await _GetCompanyAndCheckIfItExists(companyId, trackChanges);

            var companyDTO = _mapper.Map<CompanyDTO>(company);

            return companyDTO;
        }

        public async Task<IEnumerable<CompanyDTO>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
        {
            if (ids is null)
            {
                throw new IdParametersBadRequestException();
            };

            var companyEntities = await _repository.Company.GetByIdsAsync(ids, trackChanges);

            if (ids.Count() != companyEntities.Count())
            {
                throw new CollectionByIdsBadRequestException();
            };

            var companiesDTO = _mapper.Map<IEnumerable<CompanyDTO>>(companyEntities);

            return companiesDTO;
        }
        #endregion

        #region Create
        public async Task<CompanyDTO> CreateCompanyAsync(CreateCompanyDTO createCompanyDTO)
        {
            var companyEntity = _mapper.Map<Company>(createCompanyDTO);

             _repository.Company.CreateCompany(companyEntity);
            await _repository.SaveAsync();

            var companyDTO = _mapper.Map<CompanyDTO>(companyEntity);

            return companyDTO;
        }

        public async Task<(IEnumerable<CompanyDTO> companies, string ids)> CreateCompanyCollectionAsync(IEnumerable<CreateCompanyDTO> companyCollection)
        {
            if (companyCollection is null)
            {
                throw new CompanyCollectionBadRequestException();
            };

            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);

            foreach (var companyEntity in companyEntities)
            {
                _repository.Company.CreateCompany(companyEntity);
            };

            await _repository.SaveAsync();

            var companyCollectionDTO = _mapper.Map<IEnumerable<CompanyDTO>>(companyEntities);

            string ids = string.Join(",", companyCollectionDTO.Select(company => company.Id));

            return (companyCollectionDTO, ids);
        }
        #endregion

        #region Update
        public async Task UpdateCompanyAsync(Guid companyId, UpdateCompanyDTO updateCompanyDTO, bool trackChanges)
        {
            var companyEntity = await _GetCompanyAndCheckIfItExists(companyId, trackChanges);

            _mapper.Map(updateCompanyDTO, companyEntity);
            await _repository.SaveAsync();
        }
        #endregion

        #region Delete
        public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
        {
            var companyEntity = await _GetCompanyAndCheckIfItExists(companyId, trackChanges);

            _repository.Company.DeleteCompany(companyEntity);
            await _repository.SaveAsync();
        }
        #endregion

        #region Methods
        private async Task<Company> _GetCompanyAndCheckIfItExists(Guid id, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges);

            if (company is null)
                throw new CompanyNotFoundException(id);

            return company;
        }
        #endregion
    }
}
