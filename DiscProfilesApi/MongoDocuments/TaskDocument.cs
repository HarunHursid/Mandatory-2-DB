using MongoDB.Bson.Serialization.Attributes;

namespace DiscProfilesApi.MongoDocuments
{
    public class TaskDocument
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("completed")]
        public bool Completed { get; set; }

        [BsonElement("time_of_completion")]
        public DateTime? TimeOfCompletion { get; set; }

        [BsonElement("project_id")]
        public int? ProjectId { get; set; }
    }
}
