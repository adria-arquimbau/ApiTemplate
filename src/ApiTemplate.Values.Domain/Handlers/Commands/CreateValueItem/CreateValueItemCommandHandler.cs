using System;
using System.Threading;
using System.Threading.Tasks;
using ApiTemplate.Values.Domain.Entities;
using ApiTemplate.Values.Domain.Exceptions;
using ApiTemplate.Values.Domain.Proxies;
using ApiTemplate.Values.Domain.Repositories;
using MediatR;

namespace ApiTemplate.Values.Domain.Handlers.Commands.CreateValueItem
{
    public class CreateValueItemCommandHandler : IRequestHandler<CreateValueItemCommandRequest, CreateValueItemCommandResponse>
    {
        private readonly IValueItemRepository _valueItemRepository;
        private readonly INumbersProxy _numbersProxy;

        public CreateValueItemCommandHandler(IValueItemRepository valueItemRepository, INumbersProxy numbersProxy)
        {
            _valueItemRepository = valueItemRepository;
            _numbersProxy = numbersProxy;
        }   

        public async Task<CreateValueItemCommandResponse> Handle(CreateValueItemCommandRequest request, CancellationToken cancellationToken)
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

            var item = new ValueItemEntity(request.Key, valueInt);

            var valueItem = await _valueItemRepository.Get(request.Key);

           if (valueItem.HasValue) throw new ValueItemAlreadyExists();
            
            await _valueItemRepository.Create(item);
            
           return new CreateValueItemCommandResponse
           {
               Key = item.Key,
               Value = item.Value
           };
        }
    }
}