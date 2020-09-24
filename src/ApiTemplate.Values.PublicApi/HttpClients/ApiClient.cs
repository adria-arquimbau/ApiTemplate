using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ApiTemplate.Values.PublicApi.HttpClients
{
    public abstract class ApiClient
    {
        private readonly HttpClient httpClient;

        protected ApiClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpMethod method, string uri)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(httpClient.BaseAddress + uri),
                Method = method
            };
            
            return await SendWithCorrelationContext(request);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpMethod method, string uri, object objectToSend)
        {
            var jsonInString = JsonConvert.SerializeObject(objectToSend);
            var request = new HttpRequestMessage(method, httpClient.BaseAddress + uri)
            {
                Content = new StringContent(jsonInString, Encoding.UTF8, "application/json")
            };

            return await SendWithCorrelationContext(request);
        }

        private async Task<HttpResponseMessage> SendWithCorrelationContext(HttpRequestMessage request)
        {
            request.Headers.Add("Correlation-Context", Activity.Current.Baggage.Distinct().Select((p, i) => $"{p.Key}={p.Value}"));

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            // TODO: Algo pasa con el sink de Graylog (o con Graylog) que si se pone un catch o un finally el log sale por consola pero no en Graylog :(

            var response = await httpClient.SendAsync(request);

            stopwatch.Stop();

            return response;
        }
    }
}