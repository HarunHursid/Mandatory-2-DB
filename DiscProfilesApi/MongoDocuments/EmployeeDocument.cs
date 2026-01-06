using MongoDB.Bson.Serialization.Attributes;

namespace DiscProfilesApi.MongoDocuments
{
    public class EmployeeDocument
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("phone")]
        public string? Phone { get; set; }

        [BsonElement("company_id")]
        public int CompanyId { get; set; }

        [BsonElement("person_id")]
        public int? PersonId { get; set; }

        [BsonElement("department_id")]
        public int? DepartmentId { get; set; }

        [BsonElement("position_id")]
        public int? PositionId { get; set; }

        [BsonElement("disc_profile_id")]
        public int? DiscProfileId { get; set; }

        [BsonElement("created_at")]
        public DateTime? CreatedAt { get; set; }

        [BsonElement("is_active")]
        public bool? IsActive { get; set; }

        [BsonElement("last_login")]
        public DateTime? LastLogin { get; set; }
    }
}