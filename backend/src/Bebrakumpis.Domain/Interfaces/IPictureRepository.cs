using Bebrakumpis.Domain.Entities;

namespace Bebrakumpis.Domain.Interfaces;

public interface IPictureRepository
{
    Task<IEnumerable<Picture>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Picture?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetMaxOrderAsync(CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(Picture picture, CancellationToken cancellationToken = default);
    Task UpdateOrderAsync(Guid id, int order, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
