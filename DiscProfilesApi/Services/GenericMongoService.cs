using AutoMapper;
using DiscProfilesApi.Interfaces;

namespace DiscProfilesApi.Services
{
    public class GenericMongoService<TDocument, TDto> : IGenericMongoService<TDocument, TDto>
    where TDocument : class
    {
        private readonly IGenericMongoRepository<TDocument> _repository;
        private readonly IMapper _mapper;

        public GenericMongoService(
            IGenericMongoRepository<TDocument> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TDto>> GetAllAsync()
        {
            var docs = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<TDto>>(docs);
        }

        public async Task<TDto?> GetByIdAsync(int id)
        {
            var doc = await _repository.GetByIdAsync(id);
            return doc == null ? default : _mapper.Map<TDto>(doc);
        }

        public async Task<TDto> CreateAsync(TDto dto)
        {
            var doc = _mapper.Map<TDocument>(dto);
            var inserted = await _repository.InsertAsync(doc);
            return _mapper.Map<TDto>(inserted);
        }

        public async Task<bool> UpdateAsync(int id, TDto dto)
        {
            var doc = _mapper.Map<TDocument>(dto);
            return await _repository.UpdateAsync(id, doc);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
