using System.Threading;
using System.Threading.Tasks;
using ApiTemplate.Values.Domain.Entities;
using ApiTemplate.Values.Domain.Queries.GetValueItem;
using ApiTemplate.Values.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApiTemplate.Values.Domain.Handlers.Commands.CreateValueItem
{
    public class CreateValueItemHandler : IRequestHandler<CreateValueItemRequest, ValueItem>
    {
        private readonly IValueItemRepository _valueItemRepository;

        public CreateValueItemHandler(IValueItemRepository valueItemRepository)
        {
            _valueItemRepository = valueItemRepository;
        }   

        public async Task<ValueItem> Handle(CreateValueItemRequest request, CancellationToken cancellationToken)
        {
            var item = new ValueItem(request.Key, 123);

            await _valueItemRepository.Create(item);
            
           return item;
        }
    }
}
