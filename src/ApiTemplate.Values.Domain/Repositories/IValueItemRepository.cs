using System.Collections.Generic;
using System.Threading.Tasks;
using Optional;
using ApiTemplate.Values.Domain.Entities;

namespace ApiTemplate.Values.Domain.Repositories
{
    public interface IValueItemRepository
    {
        Task<Option<ValueItemEntity>> Get(string key);
        Task<IReadOnlyCollection<ValueItemEntity>> Get();
        Task Create(ValueItemEntity itemEntity);
        Task Delete(ValueItemEntity valueItemEntity);
        Task Update(ValueItemEntity valueItemEntity);
    }
}
            