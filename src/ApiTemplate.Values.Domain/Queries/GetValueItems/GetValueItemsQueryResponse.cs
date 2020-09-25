using System.Collections.Generic;
using ApiTemplate.Values.Domain.Entities;

namespace ApiTemplate.Values.Domain.Queries.GetValueItems
{
    public class GetValueItemsQueryResponse
    {

        public GetValueItemsQueryResponse(IReadOnlyCollection<ValueItemEntity> items)
        {
            Items = items;
        }

        public IReadOnlyCollection<ValueItemEntity> Items { get; }
    }
}