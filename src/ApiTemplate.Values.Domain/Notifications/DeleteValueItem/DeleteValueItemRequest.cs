using System;
using MediatR;

namespace ApiTemplate.Values.Domain.Notifications.DeleteValueItem
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
    