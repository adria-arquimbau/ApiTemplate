using ApiTemplate.Values.Domain.Entities;
using MediatR;

namespace ApiTemplate.Values.Domain.Handlers.Commands.CreateValueItem
{
    public class CreateValueItemCommandRequest : IRequest<CreateValueItemCommandResponse>
    {
        public readonly string Key;
        public readonly int Value;

        public CreateValueItemCommandRequest(string key, int value)
        {
            Key = key;
            Value = value;
        }
    }
}