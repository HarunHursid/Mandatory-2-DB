using MongoDB.Bson.Serialization.Attributes;

namespace DiscProfilesApi.MongoDocuments
{
    public class SocialEventDocument
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("disc_profile_id")]
        public int? DiscProfileId { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("company_id")]
        public int? CompanyId { get; set; }
    }
}
