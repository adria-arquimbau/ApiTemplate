using System;
using System.Threading;
using System.Threading.Tasks;
using ApiTemplate.Values.Domain.Entities;
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

        public async Task<CreateValueItemCommandResponse> Handle(CreateValueItemCommandRequest commandRequest, CancellationToken cancellationToken)
        {
            var valueInt = commandRequest.Value;

            if (commandRequest.Value == 0)
            {
                var value = await _numbersProxy.Get();
                value.Match(v =>
                {
                    valueInt = v;

                }, (() => throw new NotImplementedException()));
                
            }

            var item = new ValueItemEntity(commandRequest.Key, valueInt);

            await _valueItemRepository.Create(item);
            
           return new CreateValueItemCommandResponse
           {
               Key = item.Key,
               Value = item.Value
           };
        }
    }
}
