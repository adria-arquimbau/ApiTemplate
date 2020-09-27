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

        public async Task<Option<ValueItemEntity>> Get(string key)
        {
            var item = await _valueItemDbContext.ValueItems.FirstOrDefaultAsync(i => i.Key == key);   
            
            return item.SomeNotNull();
        }

        public async Task<IReadOnlyCollection<ValueItemEntity>> Get()
        {
            var items = await _valueItemDbContext.ValueItems.ToListAsync();
              
            return items;
        }

        public async Task Create(ValueItemEntity itemEntity)
        {
            await _valueItemDbContext.ValueItems.AddAsync(itemEntity);
            await _valueItemDbContext.SaveChangesAsync();
        }

        public async Task Delete(ValueItemEntity valueItemEntity)
        {
            _valueItemDbContext.ValueItems.Remove(valueItemEntity);
            await _valueItemDbContext.SaveChangesAsync();
        }

        public async Task Update(ValueItemEntity valueItemEntity)
        {
            _valueItemDbContext.UpdateRange(valueItemEntity);
            await _valueItemDbContext.SaveChangesAsync();
        }
    }
}