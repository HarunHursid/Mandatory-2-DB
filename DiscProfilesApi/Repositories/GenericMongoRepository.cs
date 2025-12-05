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
            // Simpel convention: CompanyDocument -> "companies", EmployeeDocument -> "employees" osv.
            var collectionName = typeof(TDocument).Name
                .Replace("Document", "")
                .ToLower() + "s";

            _collection = database.GetCollection<TDocument>(collectionName);
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
