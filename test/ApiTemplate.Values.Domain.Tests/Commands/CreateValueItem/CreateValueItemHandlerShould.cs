using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ApiTemplate.Values.Domain.Entities;
using ApiTemplate.Values.Domain.Handlers.Commands.CreateValueItem;
using ApiTemplate.Values.Domain.Queries.GetValueItem;
using ApiTemplate.Values.Domain.Repositories;
using Xunit;

namespace ApiTemplate.Values.Domain.Tests.Commands.CreateValueItem
{
    public class CreateValueItemHandlerShould
    {
        [Theory, AutoData]
        public async Task CreateItemValueIntoRepository(string key, int value)
        {
            var valueItemRepository = A.Fake<IValueItemRepository>();
            ILogger<GetValueItemHandler> logger = NullLogger<GetValueItemHandler>.Instance;
            
            var item = new ValueItem(key, value);
            
            var handler = new CreateValueItemHandler(valueItemRepository, logger);

            var response = await handler.Handle(new CreateValueItemRequest(key, value), CancellationToken.None);

            A.CallTo(() => valueItemRepository.Create(item)).MustHaveHappenedOnceExactly();
        }
    }
}
