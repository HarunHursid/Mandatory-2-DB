using AutoMapper;
using DiscProfilesApi.DTOs;
using DiscProfilesApi.Models;

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

        }
    }
}