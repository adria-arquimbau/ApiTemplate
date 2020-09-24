using System.Threading.Tasks;
using ApiTemplate.Values.PublicApi.HttpClients;
using ApiTemplate.Values.PublicApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiTemplate.Values.PublicApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IValuesApiClient valuesApiClient;
        private readonly ILogger logger;

        public ValuesController(IValuesApiClient valuesApiClient, ILogger<ValuesController> logger)
        {
            this.valuesApiClient = valuesApiClient;
            this.logger = logger;
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            logger.LogInformation("Getting key {key}", key);

            var item = await valuesApiClient.GetValueItem(key);

            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Post(ValueItem item)
        {
            await valuesApiClient.PostValueItem(item);

            return Ok(item);
        }

        [HttpPut("{key}")]
        public async Task<IActionResult> Put(string key, ValueItem item)
        {
            await valuesApiClient.PutValueItem(key, item);

            return Ok(new { key, item.Value });
        }
    }
}
