using System;
using System.Threading.Tasks;
using ApiTemplate.Values.Domain.Entities;
using ApiTemplate.Values.Infrastructure.Data.Repositories;
using AutoFixture.Xunit2;
using FluentAssertions;
using Optional;
using Xunit;

namespace ApiTemplate.Values.Infrastructure.Data.Tests.Repositories
{
    public class ValueItemRepositoryShould
    {
        private readonly ValueItemRepository _valueItemRepository;
        private readonly TestContext _testContext;

        public ValueItemRepositoryShould()
        {
            _testContext = new TestContext();
            _valueItemRepository = new ValueItemRepository(_testContext.TestDbContext());
        }

        [Theory, AutoData]
        public async Task DeleteAValueItemGivenAKey(string key)
        {
            await _testContext.RespawnDb();

            var valueItem = new ValueItemEntity(key, 12345);

            await _valueItemRepository.Create(valueItem);

            await _valueItemRepository.Delete(valueItem);

            var result = await _valueItemRepository.Get(key);

            result.HasValue.Should().BeFalse();
        }

        [Theory, AutoData]
        public async Task UpdateValueItem(string key, int value, string updatedKey, int updatedValue)
        {
            await _testContext.RespawnDb();

            await _valueItemRepository.Create(new ValueItemEntity(key, value));

            var valueItem = await _valueItemRepository.Get(key);

            await valueItem.Match(async v =>
            {
                v.Key = updatedKey;
                v.Value = updatedValue;

                await _valueItemRepository.Update(v);

            }, () =>
            {   
                throw new NotImplementedException();
            });

            
            var result = await _valueItemRepository.Get(key);

            result.HasValue.Should().BeFalse();
        }
    }
}   