using MediatR;

namespace ApiTemplate.Values.Domain.Handlers.Commands.UpdateValueItem
{
    public class UpdateValueItemCommandRequest : IRequest<UpdateValueItemCommandResponse>
    {
        public string Identifier { get; set; }
        public string Key { get; set; }
        public int Value { get; set; }
    }
}