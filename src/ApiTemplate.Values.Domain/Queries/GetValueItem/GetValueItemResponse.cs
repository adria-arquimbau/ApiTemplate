using Optional;
using ApiTemplate.Values.Domain.Entities;

namespace ApiTemplate.Values.Domain.Queries.GetValueItem
{
    public class GetValueItemResponse
    {
        public ValueItemEntity ValueItemEntity { get; }

        public GetValueItemResponse(ValueItemEntity valueItemEntity)
        {
            ValueItemEntity = valueItemEntity;
        }
    }
}
