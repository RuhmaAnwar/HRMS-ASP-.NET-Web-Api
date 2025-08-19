using AutoMapper;
using HRMS.Models;
using HRMS.Models.DTO;

namespace HRMS.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map EmployeeDTO to Employee and vice versa
            CreateMap<EmployeeDTO, Employee>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());  // Handle password separately
            //CreateMap<Employee, EmployeeDTO>()
            //    .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name));


            // Map EmployeeDTOPatch to Employee for partial updates
            CreateMap<EmployeeDTOPatch, Employee>()
                .ForMember(dest => dest.FirstName, opt =>
                    opt.Condition(src => src.FirstName != null))
                .ForMember(dest => dest.LastName, opt =>
                    opt.Condition(src => src.LastName != null))
                .ForMember(dest => dest.Email, opt =>
                    opt.Condition(src => src.Email != null))
                .ForMember(dest => dest.Role, opt =>
                    opt.Condition(src => src.Role != null))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

        }
    }
}