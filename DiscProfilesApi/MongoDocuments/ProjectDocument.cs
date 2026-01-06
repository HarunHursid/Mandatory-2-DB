using MongoDB.Bson.Serialization.Attributes;

namespace DiscProfilesApi.MongoDocuments
{
    public class ProjectDocument
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("deadline")]
        public string? Deadline { get; set; }

        [BsonElement("completed")]
        public string? Completed { get; set; }

        [BsonElement("number_of_employees")]
        public int? NumberOfEmployees { get; set; }
    }
}