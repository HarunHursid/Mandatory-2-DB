namespace DiscProfilesApi.Interfaces
{
    public interface IGenericMongoService<TDocument, TDto>
    where TDocument : class
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto?> GetByIdAsync(int id);
        Task<TDto> CreateAsync(TDto dto);
        Task<bool> UpdateAsync(int id, TDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
