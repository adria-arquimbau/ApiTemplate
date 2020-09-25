using System.Threading.Tasks;
using ApiTemplate.Values.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ApiTemplate.Values.Domain.Exceptions;
using ApiTemplate.Values.Domain.Handlers.Commands.CreateValueItem;
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
        public async Task<IActionResult> Get(string key)
        {
            var request = new GetValueItemRequest(key);

            var response = await _mediator.Send(request);

            return Ok(response.ValueItem);
        }


        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            var request = new GetValueItemsQueryRequest();

            var response = await _mediator.Send(request);

            return Ok(response.Items);
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete(string key)
        {
            var request = new DeleteValueItemRequest(key);
            await _mediator.Publish(request);
            return Ok();  
        }

        [HttpPost("{key}")]
        public async Task<IActionResult> Post(string key)
        {
            var request = new CreateValueItemRequest(key);
            var valueItem = await _mediator.Send(request);
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
