using System.Threading;
using System.Threading.Tasks;
using ApiTemplate.Values.Domain.Entities;
using ApiTemplate.Values.Domain.Notifications.DeleteValueItem;
using ApiTemplate.Values.Domain.Repositories;
using AutoFixture.Xunit2;
using NSubstitute;
using Optional;
using Xunit;

namespace ApiTemplate.Values.Domain.Tests.Commands.DeleteValueItem
{
    public class DeleteValueItemHandlerShould
    {
        private  readonly IValueItemRepository _valueItemRepository;

        public DeleteValueItemHandlerShould()
        {
            _valueItemRepository = Substitute.For<IValueItemRepository>();
        }

        [Theory, AutoData]    
        public async Task DeleteAItemWithASpecificKey(string key)
        {
            var valueItem = new ValueItem(key, 12345);

            var deleteRequest = new DeleteValueItemRequest(key);
            var handler = new DeleteValueItemNotification(_valueItemRepository);

            _valueItemRepository.Get(key).Returns(Option.Some(valueItem));

            await handler.Handle(deleteRequest, CancellationToken.None);

            await _valueItemRepository.Received(1).Get(key);
            await _valueItemRepository.Received(1).Delete(valueItem);
        }
    }
}
