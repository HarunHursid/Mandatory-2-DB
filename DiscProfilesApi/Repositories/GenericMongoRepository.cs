using DiscProfilesApi.Interfaces;
using MongoDB.Driver;


namespace DiscProfilesApi.Repositories
{
    public class GenericMongoRepository<TDocument> : IGenericMongoRepository<TDocument>
    where TDocument : class
    {

        private readonly IMongoCollection<TDocument> _collection;

        public GenericMongoRepository(IMongoDatabase database)
        {
            var collectionName = GetCollectionName(typeof(TDocument).Name);
            _collection = database.GetCollection<TDocument>(collectionName);
        }

        private static string GetCollectionName(string documentName)
        {
            // Fjern "Document" suffix og konverter til lowercase
            var baseName = documentName.Replace("Document", "").ToLower();

            // Håndter special cases
            return baseName switch
            {
                "company" => "companies",
                "discprofile" => "disc_profiles",
                "projectsdiscprofile" => "projects_disc_profiles",
                "dailytasklog" => "daily_task_logs",
                "socialevent" => "social_events",
                "stressmeasure" => "stress_measures",
                "taskevaluation" => "task_evaluations",
                _ => baseName.EndsWith("s") ? baseName : baseName + "s" // Tilføj kun 's' hvis den ikke allerede slutter på 's'
            };
        }
        public async Task<List<TDocument>> GetAllAsync() =>
            await _collection.Find(Builders<TDocument>.Filter.Empty).ToListAsync();

        public async Task<TDocument?> GetByIdAsync(int id)
        {
            var filter = Builders<TDocument>.Filter.Eq("Id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<TDocument> InsertAsync(TDocument doc)
        {
            await _collection.InsertOneAsync(doc);
            return doc;
        }

        public async Task<bool> UpdateAsync(int id, TDocument doc)
        {
            var filter = Builders<TDocument>.Filter.Eq("Id", id);
            var result = await _collection.ReplaceOneAsync(filter, doc);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var filter = Builders<TDocument>.Filter.Eq("Id", id);
            var result = await _collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

    }
}
