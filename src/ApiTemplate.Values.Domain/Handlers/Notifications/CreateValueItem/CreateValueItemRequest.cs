using MediatR;

namespace ApiTemplate.Values.Domain.Handlers.Notifications.CreateValueItem
{
    public class CreateValueItemRequest : INotification
    {
        public readonly string Key;
        public readonly int Value;

        public CreateValueItemRequest(string key, int value)
        {
            Key = key;
            Value = value;
        }
    }
}