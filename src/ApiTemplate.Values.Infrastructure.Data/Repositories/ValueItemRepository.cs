using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Optional;
using ApiTemplate.Values.Domain.Entities;
using ApiTemplate.Values.Domain.Repositories;

namespace ApiTemplate.Values.Infrastructure.Data.Repositories
{
    public class ValueItemRepository : IValueItemRepository
    {
        private readonly ValueItemDbContext _valueItemDbContext;

        public ValueItemRepository(ValueItemDbContext valueItemDbContext)
        {
            this._valueItemDbContext = valueItemDbContext;
        }

        public async Task<Option<ValueItem>> Get(string key)
        {
            var item = await _valueItemDbContext.ValueItems.FirstOrDefaultAsync(i => i.Key == key);     
            return item.SomeNotNull();
        }

        public async Task<IReadOnlyCollection<ValueItem>> Get()
        {
            var items = await _valueItemDbContext.ValueItems.ToListAsync();
            return items;
        }

        public async Task Create(ValueItem item)
        {
            await _valueItemDbContext.ValueItems.AddAsync(item);
            await _valueItemDbContext.SaveChangesAsync();
        }

        public async Task Delete(ValueItem valueItem)
        {
            _valueItemDbContext.ValueItems.Remove(valueItem);
            await _valueItemDbContext.SaveChangesAsync();
        }

        public Task Update(ValueItem valueItem)
        {
            throw new System.NotImplementedException();
        }
    }
}