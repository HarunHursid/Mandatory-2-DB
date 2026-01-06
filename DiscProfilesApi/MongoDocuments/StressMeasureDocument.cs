using MongoDB.Bson.Serialization.Attributes;

namespace DiscProfilesApi.MongoDocuments
{
    public class StressMeasureDocument
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("measure")]
        public int? Measure { get; set; }

        [BsonElement("employee_id")]
        public int? EmployeeId { get; set; }

        [BsonElement("task_id")]
        public int? TaskId { get; set; }
    }
}
