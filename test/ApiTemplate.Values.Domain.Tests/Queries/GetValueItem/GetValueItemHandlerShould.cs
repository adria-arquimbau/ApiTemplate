using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Optional;
using ApiTemplate.Values.Domain.Entities;
using ApiTemplate.Values.Domain.Queries.GetValueItem;
using ApiTemplate.Values.Domain.Repositories;
using Xunit;

namespace ApiTemplate.Values.Domain.Tests.Queries.GetValueItem
{
    public class GetValueItemHandlerShould
    {
        [Theory, AutoData]
        public async Task GetItemValueFromRepository(string key, int value)
        {
            var valueItemRepository = A.Fake<IValueItemRepository>();
            ILogger<GetValueItemHandler> logger = NullLogger<GetValueItemHandler>.Instance;

            A.CallTo(() => valueItemRepository.Get(key)).Returns(Option.Some(new ValueItemEntity(key, value)));

            var handler = new GetValueItemHandler(valueItemRepository, logger);

            var response = await handler.Handle(new GetValueItemRequest(key), CancellationToken.None);

            response.ValueItemEntity.Should().BeEquivalentTo(new ValueItemEntity(key, value));
            A.CallTo(() => valueItemRepository.Get(key)).MustHaveHappenedOnceExactly();
        }
    }
}
