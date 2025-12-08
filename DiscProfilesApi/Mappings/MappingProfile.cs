using AutoMapper;
using DiscProfilesApi.DTOs;
using DiscProfilesApi.Models;
using DiscProfilesApi.MongoDocuments;

namespace DiscProfilesApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<company, CompanyDTO>().ReverseMap();
            CreateMap<department, DepartmentDTO>().ReverseMap();
            CreateMap<daily_task_log, DailyTaskLogDTO>().ReverseMap();
            CreateMap<disc_profile, DiscProfileDTO>().ReverseMap();
            CreateMap<education, EducationDTO>().ReverseMap();
            CreateMap<employee, EmployeeDTO>().ReverseMap();
            CreateMap<person, PersonDTO>().ReverseMap();
            CreateMap<position, PositionDTO>().ReverseMap();
            CreateMap<project, ProjectDTO>().ReverseMap();
            CreateMap<projects_disc_profile, ProjectsDiscProfileDTO>().ReverseMap();
            CreateMap<social_event, SocialEventDTO>().ReverseMap();
            CreateMap<stress_measure, StressMeasureDTO>().ReverseMap();
            CreateMap<task, TaskDTO>().ReverseMap();
            CreateMap<task_evaluation, TaskEvaluationDTO>().ReverseMap();
            CreateMap<vw_SocialEventsOverview, VwSocialEventsOverviewDTO>().ReverseMap();  
            CreateMap<AppUser, AppUserDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.is_active, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.employee_id, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.created_at, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.last_login, opt => opt.MapFrom(src => src.LastLogin))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.email))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.role))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.is_active))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.employee_id))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.created_at))
                .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.last_login));

            CreateMap<CompanyDocument, CompanyDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.business_field, opt => opt.MapFrom(src => src.BusinessField))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.location))
                .ForMember(dest => dest.BusinessField, opt => opt.MapFrom(src => src.business_field));
        }
    }
}