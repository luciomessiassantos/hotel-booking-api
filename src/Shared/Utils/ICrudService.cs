namespace BookingAPI.src.Shared.Utils;

public interface ICrudService<TEntity, TCreateDto, TUpdateDto, TListDto, TResponseDto, TId>
{
    Task<TEntity> Create(TCreateDto request);
    Task<TEntity> Update(TUpdateDto request);
    Task<TResponseDto?> GetById(TId id);
    Task<PaginatedResult<TResponseDto>> List(TListDto request);
    Task SoftDelete(TId id);
    Task<TEntity> Restore(TId id);
    Task HardDelete(TId id);
}
