using MongoDB.Bson.Serialization.Attributes;

namespace DiscProfilesApi.MongoDocuments
{
    public class PersonDocument
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("private_email")]
        public string? PrivateEmail { get; set; }

        [BsonElement("private_phone")]
        public string? PrivatePhone { get; set; }

        [BsonElement("cpr")]
        public string Cpr { get; set; } = string.Empty;

        [BsonElement("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [BsonElement("last_name")]
        public string LastName { get; set; } = string.Empty;

        [BsonElement("experience")]
        public int? Experience { get; set; }

        [BsonElement("education_id")]
        public int? EducationId { get; set; }
    }
}
