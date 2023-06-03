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
        public IEnumerable<CompanyDTO> GetAllCompanies(bool trackChanges)
        {
            var companies = _repository.Company.GetAllCompanies(trackChanges);

            var companiesDTO = _mapper.Map<IEnumerable<CompanyDTO>>(companies);

            return companiesDTO;
        }
        #endregion

        #region Get By
        public CompanyDTO GetCompany(Guid id, bool trackChanges)
        {
            var company = _repository.Company.GetCompany(id, trackChanges);

            if (company == null)
            {
                throw new CompanyNotFoundException(id);
            }

            var companyDTO = _mapper.Map<CompanyDTO>(company);

            return companyDTO;
        }

        public IEnumerable<CompanyDTO> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
        {
            if (ids is null)
            {
                throw new IdParametersBadRequestException();
            };

            var companyEntities = _repository.Company.GetByIds(ids, trackChanges);

            if (ids.Count() != companyEntities.Count())
            {
                throw new CollectionByIdsBadRequestException();
            };

            var companiesDTO = _mapper.Map<IEnumerable<CompanyDTO>>(companyEntities);

            return companiesDTO;
        }
        #endregion

        #region Create
        public CompanyDTO CreateCompany(CreateCompanyDTO createCompanyDTO)
        {
            var companyEntity = _mapper.Map<Company>(createCompanyDTO);

            _repository.Company.CreateCompany(companyEntity);
            _repository.Save();

            var companyDTO = _mapper.Map<CompanyDTO>(companyEntity);

            return companyDTO;
        }

        public (IEnumerable<CompanyDTO> companies, string ids) CreateCompanyCollection(IEnumerable<CreateCompanyDTO> companyCollection)
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

            _repository.Save();

            var companyCollectionDTO = _mapper.Map<IEnumerable<CompanyDTO>>(companyEntities);

            string ids = string.Join(",", companyCollectionDTO.Select(company => company.Id));

            return (companyCollectionDTO, ids);
        }
        #endregion

        #region Update
        #endregion

        #region Delete
        #endregion
    }
}
