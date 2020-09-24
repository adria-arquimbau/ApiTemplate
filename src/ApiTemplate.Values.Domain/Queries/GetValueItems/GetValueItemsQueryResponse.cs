using System.Collections.Generic;
using ApiTemplate.Values.Domain.Entities;

namespace ApiTemplate.Values.Domain.Queries.GetValueItems
{
    public class GetValueItemsQueryResponse
    {

        public GetValueItemsQueryResponse(IReadOnlyCollection<ValueItem> items)
        {
            Items = items;
        }

        public IReadOnlyCollection<ValueItem> Items { get; }
    }
}