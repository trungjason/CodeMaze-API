﻿using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace CodeMaze_API.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Company, CompanyDTO>()
                .ForMember("FullAddress",
                    option => option.MapFrom(company => string.Join(" ", company.Address, company.Country))
                );

            CreateMap<Employee, EmployeeDTO>();

            CreateMap<CreateCompanyDTO, Company>();
            CreateMap<CreateEmployeeDTO, Employee>();
        }
    }
}
