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
        void Create(ValueItem item);
        void Delete(string key);
    }
}
    