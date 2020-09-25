using System.Threading;
using System.Threading.Tasks;
using ApiTemplate.Values.Domain.Exceptions;
using ApiTemplate.Values.Domain.Repositories;
using MediatR;

namespace ApiTemplate.Values.Domain.Handlers.Notifications.DeleteValueItem
{
    public class DeleteValueItemNotification : INotificationHandler<DeleteValueItemRequest>
    {
        private readonly IValueItemRepository _valueItemRepository;

        public DeleteValueItemNotification(IValueItemRepository valueItemRepository) 
        {
            _valueItemRepository = valueItemRepository;
        }

        public async Task Handle(DeleteValueItemRequest notification, CancellationToken cancellationToken)
        {
            var valueItem = await _valueItemRepository.Get(notification.Key);

            await valueItem.Match(async value  =>
            {
                await _valueItemRepository.Delete(value);

            }, () => throw new ValueItemNotFound());
        }
    }
}   