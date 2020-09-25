using System.Threading;
using System.Threading.Tasks;
using ApiTemplate.Values.Domain.Notifications.DeleteValueItem;
using ApiTemplate.Values.Domain.Repositories;
using AutoFixture.Xunit2;
using NSubstitute;
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
            var deleteRequest = new DeleteValueItemRequest(key);
            var handler = new DeleteValueItemHandler(_valueItemRepository);

             await handler.Handle(deleteRequest, CancellationToken.None);

            await _valueItemRepository.Received(1).Delete(key);
        }
    }
}
