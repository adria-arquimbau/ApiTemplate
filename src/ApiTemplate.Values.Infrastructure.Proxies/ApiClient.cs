using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiTemplate.Values.Infrastructure.Proxies
{
    public abstract class ApiClient
    {
        private readonly HttpClient _httpClient;
        protected ApiClient(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }
        public async Task<HttpResponseMessage> SendAsync(HttpMethod method, string uri)
        {

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(_httpClient.BaseAddress + uri),
                Method = method
            };
            return await _httpClient.SendAsync(request);
        }
        public async Task<HttpResponseMessage> SendAsync(HttpMethod method, string uri, object objectToSend)
        {
            var jsonInString = JsonConvert.SerializeObject(objectToSend);
            var request = new HttpRequestMessage(method, _httpClient.BaseAddress + uri)
            {
                Content = new StringContent(jsonInString, Encoding.UTF8, "application/json"),
            };
            return await _httpClient.SendAsync(request);
        }
    }
}
