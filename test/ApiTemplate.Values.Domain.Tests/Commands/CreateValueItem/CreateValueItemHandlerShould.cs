using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using ApiTemplate.Values.Domain.Entities;
using ApiTemplate.Values.Domain.Exceptions;
using ApiTemplate.Values.Domain.Handlers.Commands.CreateValueItem;
using ApiTemplate.Values.Domain.Proxies;
using ApiTemplate.Values.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Optional;
using Xunit;

namespace ApiTemplate.Values.Domain.Tests.Commands.CreateValueItem
{
    public class CreateValueItemHandlerShould
    {
        private readonly INumbersProxy _numbersProxy;
        private readonly IValueItemRepository _valueItemRepository;
        private readonly CreateValueItemCommandHandler _handler;

        public CreateValueItemHandlerShould()   
        {
            _numbersProxy = Substitute.For<INumbersProxy>();
            _valueItemRepository = Substitute.For<IValueItemRepository>();
            _handler = new CreateValueItemCommandHandler(_valueItemRepository, _numbersProxy);
        }

        [Theory, AutoData]
        public async Task CreateItemValueIntoRepository(string key, int value)
        {
            var item = new ValueItemEntity(key, value);
            
            await _handler.Handle(new CreateValueItemCommandRequest(key, value), CancellationToken.None);

            await _valueItemRepository.Received(1).Create(item);
        }

        [Theory, AutoData]
        public void ReturnConflictWhenYouTryToCreateAnExistingValueItemWithTheSameKey(ValueItemEntity valueItemEntity)
        {
            _valueItemRepository.Get(valueItemEntity.Key).Returns(Option.Some(valueItemEntity));

            var createValueItemCommandRequest = new CreateValueItemCommandRequest(valueItemEntity.Key, valueItemEntity.Value);

            FluentActions.Awaiting(() => _handler.Handle(createValueItemCommandRequest, CancellationToken.None)).Should().Throw<ValueItemAlreadyExists>();
        }
    }
}   