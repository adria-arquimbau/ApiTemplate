using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ApiTemplate.Values.Api.Models;
using ApiTemplate.Values.Domain.Entities;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xbehave;
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
        
        [Scenario, AutoData]
        public void SaveAnArtifact()
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotImplemented);

            
            "When we ask to save an artifact"
                .x(async () =>
                {
                    response = await _client.PostAsync($"api/v1.0/artifact/", new StringContent(JsonConvert.SerializeObject(""),
                        Encoding.UTF8,
                        "application/json"));
                });

            "Then the request was successful"
                .x(() =>
                {
                    response.StatusCode.Should().Be(HttpStatusCode.Created);
                });

            "Then the itemEntity was introduced with the right information"
                .x(async () =>
                {
                    ValueItemEntity valueItem = null;
                    
                    await _factory.ExecuteDbContextAsync(async context =>
                    {
                        valueItem = await context.ValueItems.FirstOrDefaultAsync(v => v.Key == valueItemRequest.Key);
                    });

                });
        }
    }
}