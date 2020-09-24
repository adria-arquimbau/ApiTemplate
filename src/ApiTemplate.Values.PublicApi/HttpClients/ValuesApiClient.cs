using System.Net.Http;
using System.Threading.Tasks;
using ApiTemplate.Values.PublicApi.Models;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ApiTemplate.Values.PublicApi.HttpClients
{
    public class ValuesApiClient : ApiClient, IValuesApiClient
    {
        private readonly ILogger<ValuesApiClient> logger;

        public ValuesApiClient(HttpClient httpClient, ILogger<ValuesApiClient> logger) : base(httpClient)
        {
            this.logger = logger;
        }

        public async Task<ValueItem> GetValueItem(string key)
        {
            logger.LogInformation("Calling Values Api to Get. {Key}", key);
            
            var response = await SendAsync(HttpMethod.Get, $"api/v1.0/values/{key}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ValueItem>(json);

            return result;
        }

        public async Task PutValueItem(string key, ValueItem item)
        {
            logger.LogInformation("Calling Values Api to Put. {Key} {Value}", key, item.Value);

            var response = await SendAsync(HttpMethod.Put, $"api/v1.0/values/{key}", item);

            if (response.IsSuccessStatusCode == false)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ProblemDetails>(json);

                throw new ProblemDetailsException(result);
            }
        }

        public async Task PostValueItem(ValueItem item)
        {
            logger.LogInformation("Calling Values Api to Post. {@ValueItem}", item);

            var response = await SendAsync(HttpMethod.Post, "api/v1.0/values", item);

            if (response.IsSuccessStatusCode == false)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ProblemDetails>(json);

                throw new ProblemDetailsException(result);
            }
        }
    }
}
