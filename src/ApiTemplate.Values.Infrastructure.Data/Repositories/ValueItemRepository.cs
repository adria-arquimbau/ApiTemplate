using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Optional;
using ApiTemplate.Values.Domain.Entities;
using ApiTemplate.Values.Domain.Repositories;

namespace ApiTemplate.Values.Infrastructure.Data.Repositories
{
    public class ValueItemRepository : IValueItemRepository
    {
        private readonly ValueItemDbContext context;

        public ValueItemRepository(ValueItemDbContext context)
        {
            this.context = context;
        }

        public async Task<Option<ValueItem>> Get(string key)
        {
            var item = await context.ValueItems.FirstOrDefaultAsync(i => i.Key == key);
            return item.SomeNotNull();
        }

        public async Task<IReadOnlyCollection<ValueItem>> Get()
        {
            var items = await context.ValueItems.ToListAsync();
            return items;
        }

        public async void Create(ValueItem item)
        {
            await context.ValueItems.AddAsync(item);
        }

        public void Delete(string key)
        {
            throw new System.NotImplementedException();
        }
    }
}
