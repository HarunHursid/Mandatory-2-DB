using MongoDB.Bson.Serialization.Attributes;

namespace DiscProfilesApi.MongoDocuments
{
    public class AppUserDocument
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [BsonElement("role")]
        public string Role { get; set; } = "user";

        [BsonElement("is_active")]
        public bool IsActive { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("last_login")]
        public DateTime? LastLogin { get; set; }

        [BsonElement("employee_id")]
        public int? EmployeeId { get; set; }
    }
}