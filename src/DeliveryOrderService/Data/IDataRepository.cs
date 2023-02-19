using System.Collections.Generic;
using System.Threading.Tasks;
using DeliveryOrderService.Data.Entities;

namespace DeliveryOrderService.Data;
public interface IDataRepository<T> where T : BaseEntity
{
    Task<T> AddAsync(T newEntity);
    Task<T> GetAsync(string entityId);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(string entityId);
    Task<IReadOnlyList<T>> GetAllAsync();
}
