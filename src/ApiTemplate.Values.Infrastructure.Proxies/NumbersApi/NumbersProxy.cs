using System.Net.Http;
using System.Threading.Tasks;
using ApiTemplate.Values.Domain.Exceptions;
using ApiTemplate.Values.Domain.Proxies;
using Newtonsoft.Json;
using Optional;

namespace ApiTemplate.Values.Infrastructure.Proxies.NumbersApi
{
    public class NumbersProxy : ApiClient, INumbersProxy
    {
        private readonly HttpClient _httpClient;
        const string Scheme = "Bearer";

        public NumbersProxy(HttpClient httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Option<int>> Get()
        {
            var response = await SendAsync(HttpMethod.Get, $"api/v1/number");

            if (!response.IsSuccessStatusCode)
            {
                throw new ValueItemNotFound();
            }

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<NumbersResponse>(json);

            return Option.Some(result.Number);
        }
    }
}   
