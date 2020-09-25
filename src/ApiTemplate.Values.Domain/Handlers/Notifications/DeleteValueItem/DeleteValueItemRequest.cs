using MediatR;

namespace ApiTemplate.Values.Domain.Handlers.Notifications.DeleteValueItem
{
    public class DeleteValueItemRequest : INotification
    {
        public readonly string Key;

        public DeleteValueItemRequest(string key)
        {
            Key = key;
        }
    }
}
    