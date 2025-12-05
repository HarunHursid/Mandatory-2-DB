using MongoDB.Driver;

namespace DiscProfilesApi.Interfaces
{
    public interface IGenericMongoRepository<TDocument> where TDocument : class
    {
        Task<List<TDocument>> GetAllAsync();
        Task<TDocument?> GetByIdAsync(int id);
        Task<TDocument> InsertAsync(TDocument doc);
        Task<bool> UpdateAsync(int id, TDocument doc);
        Task<bool> DeleteAsync(int id);
    

    }
}
