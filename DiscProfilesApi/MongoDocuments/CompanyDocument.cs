
using MongoDB.Bson.Serialization.Attributes;

namespace DiscProfilesApi.MongoDocuments
{
    public class CompanyDocument
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("location")]
        public string? Location { get; set; }

        [BsonElement("business_field")]
        public string? BusinessField { get; set; }
    }
}
