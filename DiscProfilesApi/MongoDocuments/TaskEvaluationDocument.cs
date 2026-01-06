using MongoDB.Bson.Serialization.Attributes;

namespace DiscProfilesApi.MongoDocuments
{
    public class TaskEvaluationDocument
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("difficulty_range")]
        public int? DifficultyRange { get; set; }

        [BsonElement("task_id")]
        public int? TaskId { get; set; }
    }
}
