using Optional;
using ApiTemplate.Values.Domain.Entities;

namespace ApiTemplate.Values.Domain.Queries.GetValueItem
{
    public class GetValueItemResponse
    {
        public Option<ValueItem> ValueItem { get; }

        public GetValueItemResponse(Option<ValueItem> valueItem)
        {
            ValueItem = valueItem;
        }
    }
}
