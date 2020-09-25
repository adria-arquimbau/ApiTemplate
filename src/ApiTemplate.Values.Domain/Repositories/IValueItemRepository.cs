using System.Collections.Generic;
using System.Threading.Tasks;
using Optional;
using ApiTemplate.Values.Domain.Entities;

namespace ApiTemplate.Values.Domain.Repositories
{
    public interface IValueItemRepository
    {
        Task<Option<ValueItem>> Get(string key);
        Task<IReadOnlyCollection<ValueItem>> Get();
        Task Create(ValueItem item);
        Task Delete(ValueItem valueItem);
        Task Update(ValueItem valueItem);
    }
}
            