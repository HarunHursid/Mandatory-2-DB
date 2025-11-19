namespace DiscProfilesApi.DTOs
{
    public class CompanyDTO
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
        public string? location { get; set; }
        public string? business_field { get; set; }
    }

    public class DepartmentDTO
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
        public int company_id { get; set; }
    }

    public class DailyTaskLogDTO
    {
        public int id { get; set; }
        public string? time_to_finish { get; set; }
        public int? task_id { get; set; }
    }

    public class DiscProfileDTO
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
        public string? color { get; set; }
        public string? description { get; set; }
    }

    public class EducationDTO
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
        public string? type { get; set; }
        public int? grade { get; set; }
    }

    public class EmployeeDTO
    {
        public int id { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public int company_id { get; set; }
        public int? person_id { get; set; }
        public int? department_id { get; set; }
        public int? position_id { get; set; }
        public int? disc_profile_id { get; set; }
    }

    public class PersonDTO
    {
        public int id { get; set; }
        public string? private_email { get; set; }
        public string? private_phone { get; set; }
        public string? cpr { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public int? experience { get; set; }
        public int? EducationID { get; set; }
    }

    public class PositionDTO
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
    }

    public class ProjectDTO
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
        public string? description { get; set; }
        public string? deadline { get; set; }
        public string? completed { get; set; }
        public int? number_of_employees { get; set; }
    }

    public class ProjectsDiscProfileDTO
    {
        public int id { get; set; }
        public int project_id { get; set; }
        public int disc_profile_id { get; set; }
    }

    public class SocialEventDTO
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
        public int? disc_profile_id { get; set; }
        public string? description { get; set; }
        public int? company_id { get; set; }
    }

    public class StressMeasureDTO
    {
        public int id { get; set; }
        public string? description { get; set; }
        public int? measure { get; set; }
        public int? employee_id { get; set; }
        public int? task_id { get; set; }
    }

    public class TaskDTO
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
        public int? project_id { get; set; }
        
        public bool completed { get; set; }
        public DateTime? time_of_completion { get; set; }
    }

    public class TaskEvaluationDTO
    {
        public int id { get; set; }
        public string? description { get; set; }
        public int? difficulty_range { get; set; }
        public int? task_id { get; set; }
    }
    public class VwSocialEventsOverviewDTO
    {
        public int social_event_id { get; set; }
        public string event_name { get; set; } = null!;
        public string? event_description { get; set; }
        public string? company_name { get; set; }
        public string? disc_profile_name { get; set; }
        public string? disc_color { get; set; }
        public string? disc_description { get; set; }
    }
}