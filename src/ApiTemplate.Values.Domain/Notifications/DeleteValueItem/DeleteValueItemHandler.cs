using System;
using System.Threading;
using System.Threading.Tasks;
using ApiTemplate.Values.Domain.Repositories;
using MediatR;

namespace ApiTemplate.Values.Domain.Notifications.DeleteValueItem
{
    public class DeleteValueItemHandler : INotificationHandler<DeleteValueItemRequest>
    {
        private readonly IValueItemRepository _valueItemRepository;

        public DeleteValueItemHandler(IValueItemRepository valueItemRepository) 
        {
            _valueItemRepository = valueItemRepository;
        }

        public async Task Handle(DeleteValueItemRequest notification, CancellationToken cancellationToken)
        {
            _valueItemRepository.Delete(notification.Key);
        }
    }
}