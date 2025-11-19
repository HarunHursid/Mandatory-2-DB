namespace DiscProfilesApi.DTOs
{
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
}
