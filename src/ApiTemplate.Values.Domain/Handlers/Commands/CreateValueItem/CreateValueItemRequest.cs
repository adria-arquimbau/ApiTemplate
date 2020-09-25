using ApiTemplate.Values.Domain.Entities;
using MediatR;

namespace ApiTemplate.Values.Domain.Handlers.Commands.CreateValueItem
{
    public class CreateValueItemRequest : IRequest<CreateValueItemResponse>
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