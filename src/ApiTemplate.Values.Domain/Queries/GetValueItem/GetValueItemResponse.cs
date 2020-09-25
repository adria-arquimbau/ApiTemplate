using Optional;
using ApiTemplate.Values.Domain.Entities;

namespace ApiTemplate.Values.Domain.Queries.GetValueItem
{
    public class GetValueItemResponse
    {
        public ValueItem ValueItem { get; }

        public GetValueItemResponse(ValueItem valueItem)
        {
            ValueItem = valueItem;
        }
    }
}
