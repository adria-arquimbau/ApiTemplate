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
        private readonly IValueItemRepository _valueItemRepository;

        public GetValueItemHandler(IValueItemRepository valueItemRepository, ILogger<GetValueItemHandler> logger)
        {
            _valueItemRepository = valueItemRepository;
        }
        public async Task<GetValueItemResponse> Handle(GetValueItemRequest request, CancellationToken cancellationToken)
        {
            var valueItem = await _valueItemRepository.Get(request.Key);

            return valueItem.Match( item =>
            {
                return new GetValueItemResponse(item);

            }, () => throw new ValueItemNotFound());

        }
    }
}
