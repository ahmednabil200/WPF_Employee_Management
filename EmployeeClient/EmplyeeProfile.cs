using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain;

namespace Client;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<EmployeeView, Employee>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
    }
}