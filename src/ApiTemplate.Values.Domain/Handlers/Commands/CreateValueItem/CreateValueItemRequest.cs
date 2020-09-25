using ApiTemplate.Values.Domain.Entities;
using MediatR;

namespace ApiTemplate.Values.Domain.Handlers.Commands.CreateValueItem
{
    public class CreateValueItemRequest : IRequest<ValueItem>
    {
        public readonly string Key;

        public CreateValueItemRequest(string key)
        {
            Key = key;
        }
    }
}