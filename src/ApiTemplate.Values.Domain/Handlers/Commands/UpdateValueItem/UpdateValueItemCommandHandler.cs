using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApiTemplate.Values.Domain.Entities;
using ApiTemplate.Values.Domain.Exceptions;
using ApiTemplate.Values.Domain.Repositories;
using MediatR;

namespace ApiTemplate.Values.Domain.Handlers.Commands.UpdateValueItem
{
    public class UpdateValueItemCommandHandler : IRequestHandler<UpdateValueItemCommandRequest, UpdateValueItemCommandResponse>
    {
        private readonly IValueItemRepository _valueItemRepository;

        public UpdateValueItemCommandHandler(IValueItemRepository valueItemRepository)
        {
            _valueItemRepository = valueItemRepository;
        }
            
        public async Task<UpdateValueItemCommandResponse> Handle(UpdateValueItemCommandRequest request, CancellationToken cancellationToken)
        {
            var valueItem = await _valueItemRepository.Get(request.Identifier);

            return await valueItem.Match(async v =>
            {
                    
                await _valueItemRepository.Update(new ValueItem(request.Key, request.Value));

                return new UpdateValueItemCommandResponse
                {
                    Key = request.Key,
                    Value = request.Value
                };

            },(() => throw new ValueItemNotFound()));
        }
    }   
}