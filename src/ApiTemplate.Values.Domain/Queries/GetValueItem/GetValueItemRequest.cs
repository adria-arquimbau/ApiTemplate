using MediatR;

namespace ApiTemplate.Values.Domain.Queries.GetValueItem
{
    public class GetValueItemRequest : IRequest<GetValueItemResponse>
    {
        public string Key { get;  }

        public GetValueItemRequest(string key)
        {
            Key = key;
        }
    }
}
