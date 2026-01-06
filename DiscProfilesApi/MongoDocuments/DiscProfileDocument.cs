using MongoDB.Bson.Serialization.Attributes;

namespace DiscProfilesApi.MongoDocuments
{
    public class DiscProfileDocument
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("color")]
        public string? Color { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }
    }
}