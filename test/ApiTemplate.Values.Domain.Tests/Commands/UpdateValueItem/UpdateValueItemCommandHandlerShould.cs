using System.Threading;
using System.Threading.Tasks;
using ApiTemplate.Values.Domain.Entities;
using ApiTemplate.Values.Domain.Handlers.Commands.UpdateValueItem;
using ApiTemplate.Values.Domain.Repositories;
using AutoFixture.Xunit2;
using FluentAssertions;
using NSubstitute;
using Optional;
using Xunit;

namespace ApiTemplate.Values.Domain.Tests.Commands.UpdateValueItem
{
    public class UpdateValueItemCommandHandlerShould
    {
        private readonly IValueItemRepository _valueItemRepository;

        public UpdateValueItemCommandHandlerShould()
        {
            _valueItemRepository = Substitute.For<IValueItemRepository>();
        }

        [Theory, AutoData]
        public async Task UpdateValueItem(string key, string keyIdentifier, int value)
        {
            var valueItem = new ValueItemEntity(key, value);

            var deleteRequest = new UpdateValueItemCommandRequest
            {
                Identifier = keyIdentifier,
                Key = key,
                Value = value
            };

            _valueItemRepository.Get(keyIdentifier).Returns(Option.Some(valueItem));

            var handler = new UpdateValueItemCommandHandler(_valueItemRepository);

            var response = await handler.Handle(deleteRequest, CancellationToken.None);

            await _valueItemRepository.Received(1).Get(keyIdentifier);
            await _valueItemRepository.Received(1).Update(valueItem);  

            response.Key.Should().Be(key);
            response.Value.Should().Be(value);
        }
    }
}
