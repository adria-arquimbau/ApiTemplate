using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ApiTemplate.Values.Api.Tests.Controllers
{
    public class ArtifactControllerShould : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public ArtifactControllerShould(  
            CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {   
                AllowAutoRedirect = false
            });
            Task.WaitAll(_factory.RespawnDbContext());
        }
    }
}