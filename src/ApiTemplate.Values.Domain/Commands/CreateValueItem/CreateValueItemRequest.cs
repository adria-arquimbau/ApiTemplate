using MediatR;
using ApiTemplate.Values.Domain.Entities;

namespace ApiTemplate.Values.Domain.Commands.CreateValueItem
{
    public class CreateValueItemRequest : IRequest<ValueItem>
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