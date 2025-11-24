using DiscProfilesApi.Interfaces;
using AutoMapper;

namespace DiscProfilesApi.Services;

public class GenericService<TEntity, TDto> : IGenericService<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    protected readonly IGenericRepository<TEntity> _repository;
    protected readonly IMapper _mapper;

    public GenericService(IGenericRepository<TEntity> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public virtual async Task<IEnumerable<TDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<TDto>>(entities);
    }

    public virtual async Task<TDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<TDto>(entity);
    }

    public virtual async Task<TDto> CreateAsync(TDto dto)
    {
        var entity = _mapper.Map<TEntity>(dto);
        var created = await _repository.AddAsync(entity);
        return _mapper.Map<TDto>(created);
    }

    public virtual async Task<TDto?> UpdateAsync(int id, TDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        _mapper.Map(dto, existing);
        var updated = await _repository.UpdateAsync(existing);
        return _mapper.Map<TDto>(updated);
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public virtual async Task<TEntity?> GetEntityByEmailAsync(string email)
    {
        return await _repository.GetByEmailAsync(email);
    }
}