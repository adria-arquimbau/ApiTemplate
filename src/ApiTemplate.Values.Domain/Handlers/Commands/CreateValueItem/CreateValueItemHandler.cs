using System;
using System.Threading;
using System.Threading.Tasks;
using ApiTemplate.Values.Domain.Entities;
using ApiTemplate.Values.Domain.Proxies;
using ApiTemplate.Values.Domain.Queries.GetValueItem;
using ApiTemplate.Values.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApiTemplate.Values.Domain.Handlers.Commands.CreateValueItem
{
    public class CreateValueItemHandler : IRequestHandler<CreateValueItemRequest, CreateValueItemResponse>
    {
        private readonly IValueItemRepository _valueItemRepository;
        private readonly INumbersProxy _numbersProxy;

        public CreateValueItemHandler(IValueItemRepository valueItemRepository, INumbersProxy numbersProxy)
        {
            _valueItemRepository = valueItemRepository;
            _numbersProxy = numbersProxy;
        }   

        public async Task<CreateValueItemResponse> Handle(CreateValueItemRequest request, CancellationToken cancellationToken)
        {
            var valueInt = request.Value;

            if (request.Value == 0)
            {
                var value = await _numbersProxy.Get();
                value.Match(v =>
                {
                    valueInt = v;

                }, (() => throw new NotImplementedException()));
                
            }

            var item = new ValueItem(request.Key, valueInt);

            await _valueItemRepository.Create(item);
            
           return new CreateValueItemResponse
           {
               Key = item.Key,
               Value = item.Value
           };
        }
    }
}
