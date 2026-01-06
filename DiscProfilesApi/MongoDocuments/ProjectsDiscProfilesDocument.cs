using MongoDB.Bson.Serialization.Attributes;

namespace DiscProfilesApi.MongoDocuments
{
    public class ProjectsDiscProfilesDocument
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("project_id")]
        public int ProjectId { get; set; }

        [BsonElement("disc_profile_id")]
        public int DiscProfileId { get; set; }
    }
}
