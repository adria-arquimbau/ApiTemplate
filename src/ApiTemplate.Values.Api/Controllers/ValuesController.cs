﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ApiTemplate.Values.Api.Models;
using ApiTemplate.Values.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ApiTemplate.Values.Domain.Exceptions;
using ApiTemplate.Values.Domain.Handlers.Commands.CreateValueItem;
using ApiTemplate.Values.Domain.Handlers.Commands.UpdateValueItem;
using ApiTemplate.Values.Domain.Handlers.Notifications.DeleteValueItem;
using ApiTemplate.Values.Domain.Queries.GetValueItem;
using ApiTemplate.Values.Domain.Queries.GetValueItems;

namespace ApiTemplate.Values.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IMediator _mediator;

        public ValuesController(IMediator mediator)
        {
            this._mediator = mediator;
        }   

        [HttpGet("{key}")]
        [ProducesResponseType(typeof(ValueItemResponse),(int)HttpStatusCode.OK)]
        [Produces("application/json")]
        public async Task<IActionResult> Get(string key)
        {
            var request = new GetValueItemRequest(key);

            var response = await _mediator.Send(request);

            return Ok(response.ValueItemEntity);
        }


        [HttpGet()]
        [ProducesResponseType(typeof(IReadOnlyCollection<ValueItemEntity>), (int)HttpStatusCode.OK)]
        [Produces("application/json")]
        public async Task<IActionResult> Get()
        {
            var request = new GetValueItemsQueryRequest();

            var response = await _mediator.Send(request);

            return Ok(response.Items);
        }

        [HttpDelete("{key}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Produces("application/json")]
        public async Task<IActionResult> Delete(string key)
        {
            var request = new DeleteValueItemRequest(key);
            await _mediator.Publish(request);
            return Ok();  
        }

        [HttpPost("{key}/{value}")]
        [ProducesResponseType(typeof(ValueItemResponse), (int)HttpStatusCode.Created)]
        [Produces("application/json")]
        public async Task<IActionResult> Post(string key, int value)
        {
            var request = new CreateValueItemCommandRequest(key, value);
            var valueItem = await _mediator.Send(request);
            return Created("" ,new ValueItemResponse
            {
                Key = valueItem.Key,
                Value = valueItem.Value
            });
        }

        [HttpPut("{key}")]
        [ProducesResponseType(typeof(ValueItemResponse), (int)HttpStatusCode.OK)]
        [Produces("application/json")]
        public async Task<IActionResult> Put([FromRoute]string key, [FromBody]ValueItemRequest item)
        {
            if (key.Length < 5) throw new KeyTooShortException("The key is too short.", key, key.Length);

            var request = new UpdateValueItemCommandRequest
            {
                Identifier = key,
                Key = item.Key,
                Value = item.Value
            };

            var response = await _mediator.Send(request);

            return Ok(new ValueItemResponse
            {
                Value = response.Value,
                Key = response.Key
            });
        }
    }
}
