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
            // SQL til DTO mappings
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
            
            // AppUser til DTO mapping
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

            // MongoDB Document til DTO mappings
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

            // DiscProfileDocument til DTO mapping
            CreateMap<DiscProfileDocument, DiscProfileDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.color, opt => opt.MapFrom(src => src.Color))
                .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Description))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.color))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description));

            // EmployeeDocument til DTO mapping
            CreateMap<EmployeeDocument, EmployeeDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.company_id, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.person_id, opt => opt.MapFrom(src => src.PersonId))
                .ForMember(dest => dest.department_id, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.position_id, opt => opt.MapFrom(src => src.PositionId))
                .ForMember(dest => dest.disc_profile_id, opt => opt.MapFrom(src => src.DiscProfileId))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.phone))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.company_id))
                .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => src.person_id))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.department_id))
                .ForMember(dest => dest.PositionId, opt => opt.MapFrom(src => src.position_id))
                .ForMember(dest => dest.DiscProfileId, opt => opt.MapFrom(src => src.disc_profile_id));

            // ProjectDocument til DTO mapping
            CreateMap<ProjectDocument, ProjectDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.deadline, opt => opt.MapFrom(src => src.Deadline))
                .ForMember(dest => dest.completed, opt => opt.MapFrom(src => src.Completed))
                .ForMember(dest => dest.number_of_employees, opt => opt.MapFrom(src => src.NumberOfEmployees))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description))
                .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.deadline))
                .ForMember(dest => dest.Completed, opt => opt.MapFrom(src => src.completed))
                .ForMember(dest => dest.NumberOfEmployees, opt => opt.MapFrom(src => src.number_of_employees));

            // DepartmentDocument til DTO mapping
            CreateMap<DepartmentDocument, DepartmentDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.company_id, opt => opt.MapFrom(src => src.CompanyId))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.company_id));

            // EducationDocument til DTO mapping
            CreateMap<EducationDocument, EducationDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.grade, opt => opt.MapFrom(src => src.Grade))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.type))
                .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.grade));

            // PersonDocument til DTO mapping
            CreateMap<PersonDocument, PersonDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.private_email, opt => opt.MapFrom(src => src.PrivateEmail))
                .ForMember(dest => dest.private_phone, opt => opt.MapFrom(src => src.PrivatePhone))
                .ForMember(dest => dest.cpr, opt => opt.MapFrom(src => src.Cpr))
                .ForMember(dest => dest.first_name, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.last_name, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.experience, opt => opt.MapFrom(src => src.Experience))
                .ForMember(dest => dest.EducationID, opt => opt.MapFrom(src => src.EducationId))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.PrivateEmail, opt => opt.MapFrom(src => src.private_email))
                .ForMember(dest => dest.PrivatePhone, opt => opt.MapFrom(src => src.private_phone))
                .ForMember(dest => dest.Cpr, opt => opt.MapFrom(src => src.cpr))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.first_name))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.last_name))
                .ForMember(dest => dest.Experience, opt => opt.MapFrom(src => src.experience))
                .ForMember(dest => dest.EducationId, opt => opt.MapFrom(src => src.EducationID));

            // PositionDocument til DTO mapping
            CreateMap<PositionDocument, PositionDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name));

            // TaskDocument til DTO mapping
            CreateMap<TaskDocument, TaskDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.completed, opt => opt.MapFrom(src => src.Completed))
                .ForMember(dest => dest.time_of_completion, opt => opt.MapFrom(src => src.TimeOfCompletion))
                .ForMember(dest => dest.project_id, opt => opt.MapFrom(src => src.ProjectId))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.Completed, opt => opt.MapFrom(src => src.completed))
                .ForMember(dest => dest.TimeOfCompletion, opt => opt.MapFrom(src => src.time_of_completion))
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.project_id));

            // SocialEventDocument til DTO mapping
            CreateMap<SocialEventDocument, SocialEventDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.disc_profile_id, opt => opt.MapFrom(src => src.DiscProfileId))
                .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.company_id, opt => opt.MapFrom(src => src.CompanyId))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.DiscProfileId, opt => opt.MapFrom(src => src.disc_profile_id))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.company_id));

            // DailyTaskLogDocument til DTO mapping
            CreateMap<DailyTaskLogDocument, DailyTaskLogDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.time_to_finish, opt => opt.MapFrom(src => src.TimeToComplete))
                .ForMember(dest => dest.task_id, opt => opt.MapFrom(src => src.TaskId))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.TimeToComplete, opt => opt.MapFrom(src => src.time_to_finish))
                .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.task_id));

            // TaskEvaluationDocument til DTO mapping
            CreateMap<TaskEvaluationDocument, TaskEvaluationDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.difficulty_range, opt => opt.MapFrom(src => src.DifficultyRange))
                .ForMember(dest => dest.task_id, opt => opt.MapFrom(src => src.TaskId))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description))
                .ForMember(dest => dest.DifficultyRange, opt => opt.MapFrom(src => src.difficulty_range))
                .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.task_id));

            // StressMeasureDocument til DTO mapping
            CreateMap<StressMeasureDocument, StressMeasureDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.measure, opt => opt.MapFrom(src => src.Measure))
                .ForMember(dest => dest.employee_id, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.task_id, opt => opt.MapFrom(src => src.TaskId))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description))
                .ForMember(dest => dest.Measure, opt => opt.MapFrom(src => src.measure))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.employee_id))
                .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.task_id));

            // ProjectsDiscProfilesDocument til DTO mapping
            CreateMap<ProjectsDiscProfilesDocument, ProjectsDiscProfileDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.project_id, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dest => dest.disc_profile_id, opt => opt.MapFrom(src => src.DiscProfileId))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.project_id))
                .ForMember(dest => dest.DiscProfileId, opt => opt.MapFrom(src => src.disc_profile_id));
        }
    }
}