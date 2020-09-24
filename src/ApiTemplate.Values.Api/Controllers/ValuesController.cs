using System;
using System.Threading.Tasks;
using ApiTemplate.Values.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ApiTemplate.Values.Domain.Commands.CreateValueItem;
using ApiTemplate.Values.Domain.Exceptions;
using ApiTemplate.Values.Domain.Notifications.DeleteValueItem;
using ApiTemplate.Values.Domain.Queries.GetValueItem;
using ApiTemplate.Values.Domain.Queries.GetValueItems;

namespace ApiTemplate.Values.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IMediator mediator;

        public ValuesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            var request = new GetValueItemRequest(key);

            var response = await mediator.Send(request);

            return response.ValueItem.Match(r => (IActionResult)Ok(new ValueItem(r)), () => (IActionResult)NotFound(key));
        }


        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            var request = new GetValueItemsQueryRequest();

            var response = await mediator.Send(request);

            return Ok(response.Items);
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete(string key)
        {
            var request = new DeleteValueItemRequest(key);
            var response = await mediator.Send(request);
            return Ok();  
        }

        [HttpPost()]
        public async Task<IActionResult> Post(ValueItem item)
        {
            var request = new CreateValueItemRequest(item.Key, item.Value);
            var valueItem = await mediator.Send(request);
            return Ok(valueItem);
        }

        [HttpPut("{key}")]
        public async Task<IActionResult> Put(string key, ValueItem item)
        {
            if (key.Length > 5) throw new KeyTooLongException("The key is too long.", key, key.Length);

            return Ok(new {key, item.Value});
        }
    }
}
