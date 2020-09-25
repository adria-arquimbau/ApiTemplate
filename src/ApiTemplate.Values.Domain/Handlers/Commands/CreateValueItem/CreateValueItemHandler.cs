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

            await valueItemRepository.Create(item);
            
           return item;
        }
    }
}
