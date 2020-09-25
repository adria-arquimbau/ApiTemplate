using System.Threading;
using System.Threading.Tasks;
using ApiTemplate.Values.Domain.Exceptions;
using ApiTemplate.Values.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApiTemplate.Values.Domain.Queries.GetValueItem
{
    public class GetValueItemHandler : IRequestHandler<GetValueItemRequest, GetValueItemResponse>
    {
        private readonly IValueItemRepository valueItemRepository;
        private readonly ILogger<GetValueItemHandler> logger;

        public GetValueItemHandler(IValueItemRepository valueItemRepository, ILogger<GetValueItemHandler> logger)
        {
            this.valueItemRepository = valueItemRepository;
            this.logger = logger;
        }
        public async Task<GetValueItemResponse> Handle(GetValueItemRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Test logging {@Request}", request); // this should go in a behavior
            var valueItem = await valueItemRepository.Get(request.Key);

            return valueItem.Match( item =>
            {
                return new GetValueItemResponse(item);

            }, () => throw new ValueItemNotFound());

        }
    }
}
