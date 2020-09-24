using System.Threading;
using System.Threading.Tasks;
using ApiTemplate.Values.Domain.Queries.GetValueItem;
using ApiTemplate.Values.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using ApiTemplate.Values.Domain.Entities;

namespace ApiTemplate.Values.Domain.Commands.CreateValueItem
{
    public class CreateValueItemHandler : IRequestHandler<CreateValueItemRequest, ValueItem>
    {
        private readonly IValueItemRepository valueItemRepository;
        private readonly ILogger<GetValueItemHandler> logger;

        public CreateValueItemHandler(IValueItemRepository valueItemRepository, ILogger<GetValueItemHandler> logger)
        {
            this.valueItemRepository = valueItemRepository;
            this.logger = logger;
        }

        public async Task<ValueItem> Handle(CreateValueItemRequest request, CancellationToken cancellationToken)
        {
            var item = new ValueItem(request.Key, request.Value);

            valueItemRepository.Create(item);
            
           return item;
        }
    }
}
