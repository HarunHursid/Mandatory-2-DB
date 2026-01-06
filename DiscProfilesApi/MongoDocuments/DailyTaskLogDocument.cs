using MongoDB.Bson.Serialization.Attributes;

namespace DiscProfilesApi.MongoDocuments
{
    public class DailyTaskLogDocument
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("time_to_complete")]
        public string? TimeToComplete { get; set; }

        [BsonElement("task_id")]
        public int? TaskId { get; set; }
    }
}
