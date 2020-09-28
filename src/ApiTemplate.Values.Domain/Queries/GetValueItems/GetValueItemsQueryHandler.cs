using System;
using System.Threading;
using System.Threading.Tasks;
using ApiTemplate.Values.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApiTemplate.Values.Domain.Queries.GetValueItems
{
    public class GetValueItemsQueryHandler : IRequestHandler<GetValueItemsQueryRequest, GetValueItemsQueryResponse>
    {
        private readonly IValueItemRepository _itemRepository;

        public GetValueItemsQueryHandler(IValueItemRepository itemRepository, ILogger<GetValueItemsQueryHandler> logger)
        {
            _itemRepository = itemRepository;
        }

        public async Task<GetValueItemsQueryResponse> Handle(GetValueItemsQueryRequest request, CancellationToken cancellationToken)
        {
            var items = await _itemRepository.Get();

            return new GetValueItemsQueryResponse(items);
        }
    }
}