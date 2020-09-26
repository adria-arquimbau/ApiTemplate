using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ApiTemplate.Values.Domain.Entities;
using ApiTemplate.Values.Domain.Handlers.Commands.CreateValueItem;
using ApiTemplate.Values.Domain.Proxies;
using ApiTemplate.Values.Domain.Queries.GetValueItem;
using ApiTemplate.Values.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace ApiTemplate.Values.Domain.Tests.Commands.CreateValueItem
{
    public class CreateValueItemHandlerShould
    {
        private readonly INumbersProxy _numbersProxy;

        public CreateValueItemHandlerShould()
        {
            _numbersProxy = Substitute.For<INumbersProxy>();
        }

        [Theory, AutoData]
        public async Task CreateItemValueIntoRepository(string key, int value)
        {
            var valueItemRepository = A.Fake<IValueItemRepository>();
            ILogger<GetValueItemHandler> logger = NullLogger<GetValueItemHandler>.Instance;
            
            var item = new ValueItemEntity(key, value);
            
            var handler = new CreateValueItemCommandHandler(valueItemRepository, _numbersProxy);

            var response = await handler.Handle(new CreateValueItemCommandRequest(key, value), CancellationToken.None);

            A.CallTo(() => valueItemRepository.Create(item)).MustHaveHappenedOnceExactly();
        }
    }
}
    