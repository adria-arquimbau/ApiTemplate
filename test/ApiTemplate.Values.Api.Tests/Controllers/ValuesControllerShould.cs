using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using ApiTemplate.Values.Api.Models;
using ApiTemplate.Values.Domain.Entities;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xbehave;
using Xunit;

namespace ApiTemplate.Values.Api.Tests.Controllers
{
    public class ValuesControllerShould :
        IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public ValuesControllerShould(  
            CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {   
                AllowAutoRedirect = false
            });
        }

        [Scenario, AutoData]
        public void CreateAValueItemGivenAKeyAndValue()
        {
            var valueItem = new ValueItemResponse
            {
                Value = 123,
                Key = "abc"
            };

            var response = new HttpResponseMessage(HttpStatusCode.NotImplemented);

            "Deleting all items on the data base"
                .x(async () =>
                {   
                    await _factory.RespawnDbContext();
                });

            "When we ask to create the itemEntity"
                .x(async () =>
                {
                    response = await _client.PostAsync($"api/v1.0/values/{valueItem.Key}/{valueItem.Value}", null);
                });

            "Then the response was successful"
                .x(async () =>
                {
                    response.StatusCode.Should().BeEquivalentTo(HttpStatusCode.Created);
                });

            "Then the itemEntity is returned with the right information"
                .x(async () =>
                {
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ValueItemResponse>(json);

                    result.Should().BeEquivalentTo(valueItem);
                });
        }

        [Scenario, AutoData]
        public void CreateAValueItemGivenAKeyAndValue0(string key)
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotImplemented);

            var server = WireMockServer.Start(10800);

            "Deleting all items on the data base"
                .x(async () =>
                {
                    await _factory.RespawnDbContext();
                });

            "When we ask to create the itemEntity"
                .x(async () =>
                {
                    server
                        .Given(Request
                            .Create()
                            .WithPath("/api/v1/number")
                            .UsingGet())
                        .RespondWith(Response.Create().WithBodyAsJson(new
                        {
                            Number = 1,
                        }));

                    response = await _client.PostAsync($"api/v1.0/values/{key}/0", null);
                });

            "Then the response was successful"
                .x(async () =>
                {
                    response.StatusCode.Should().BeEquivalentTo(HttpStatusCode.Created);
                });

            "Then the itemEntity is returned with the right information"
                .x(async () =>
                {
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ValueItemEntity>(json);

                    result.Key.Should().Be(key);
                    result.Value.Should().Be(1);
                    server.Stop();
                });
        }

        [Scenario, AutoData]
        public void GetValuesWithKey(string key, int value)
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotImplemented);

            "Given an itemEntity is in the database"
                .x(async () =>
                {
                    var item = new ValueItemEntity(key, value);
                    await _factory.RespawnDbContext();
                    await _factory.ExecuteDbContextAsync(async context =>
                    {
                        await context.ValueItems.AddAsync(item);
                        await context.SaveChangesAsync();
                    });
                });

            "When we ask for that itemEntity through the API"
                .x(async () =>
                {
                    response = await _client.GetAsync($"api/v1.0/values/{key}");
                });

            "Then the itemEntity is returned with the right information"
                .x(async () =>
                {
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ValueItemEntity>(json);

                    result.Key.Should().Be(key);
                    result.Value.Should().Be(value);
                });
        }

        [Scenario, AutoData]
        public void ReturnAllItems(List<ValueItemEntity> valueItems)
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotImplemented);

            "Given there are items in the database"
                .x(async () =>
                {
                    await _factory.RespawnDbContext();
                    await _factory.ExecuteDbContextAsync(async context =>
                    {
                        await context.ValueItems.AddRangeAsync(valueItems);
                        await context.SaveChangesAsync();
                    });
                });

            "When we ask for that itemEntity through the API"
                .x(async () =>
                {
                    response = await _client.GetAsync($"api/v1.0/values/");
                });

            "Then all the items are returned with the right informaiton"
                .x(async () =>
                {
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<ValueItemEntity>>(json);

                    result.Should().BeEquivalentTo(valueItems);
                });
        }

        [Scenario, AutoData]
        public void ReturnNotFoundIfTheKeyIsNotInTheSystem(string key, int value)
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotImplemented);

            "Given an itemEntity is not in the database"
                .x(async () =>
                {
                    await _factory.RespawnDbContext();
                });

            "When we ask for that itemEntity through the API"
                .x(async () =>
                {
                    response = await _client.GetAsync($"api/v1.0/values/{key}");
                });

            "Then the API returns NotFound"
                .x(async () =>
                {
                    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
                });
        }

        [Scenario, AutoData]
        public void DeleteASpecificValueItem(string key, int value)
        {
            var deletedResponse = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            var getResponse = new HttpResponseMessage(HttpStatusCode.NotImplemented);

            "Given an itemEntity is in the database"
                .x(async () =>
                {
                    var item = new ValueItemEntity(key, value);
                    await _factory.RespawnDbContext();
                    await _factory.ExecuteDbContextAsync(async context =>
                    {
                        await context.ValueItems.AddAsync(item);
                        await context.SaveChangesAsync();
                    });
                });

            "When we ask for delete the specific itemEntity"
                .x(async () =>
                {
                    deletedResponse = await _client.DeleteAsync($"api/v1.0/values/{key}");
                });

            "Then the request was successful"
                .x(async () =>
                {
                    deletedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
                });

            "Then the value itemEntity was deleted when we ask for the specific deleted itemEntity"   
                .x(async () =>
                {
                    getResponse = await _client.GetAsync($"api/v1.0/values/{key}");
                });

            "Then the value itemEntity not exists"
                .x(async () =>
                {
                    getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
                });
        }

        [Scenario, AutoData]
        public void SayValueItemNotFoundIfYouTryToDeleteAnUnexistingValueItem(string key)
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotImplemented);

            "Delete all items in the database"
                .x(async () =>
                {
                    await _factory.RespawnDbContext();
                });

            "When we ask for delete the specific itemEntity"
                .x(async () =>
                {
                    response = await _client.DeleteAsync($"api/v1.0/values/{key}");
                });

            "Then the response was not found"
                .x(async () =>
                {
                    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
                });
        }

        [Scenario, AutoData]
        public void UpdateAValueItemGivenAKey(string key, int value, string keyToUpdate, int valueToUpdate)
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            var request = new ValueItemRequest
            {
                Key = keyToUpdate,
                Value = valueToUpdate
            };
                
            "Deleting all items on the data base"
                .x(async () =>
                {
                    var item = new ValueItemEntity(key, value);
                    await _factory.RespawnDbContext();
                    await _factory.ExecuteDbContextAsync(async context =>
                    {
                        await context.ValueItems.AddAsync(item);
                        await context.SaveChangesAsync();
                    });
                });

            "When we ask to update an existing itemEntity"
                .x(async () =>
                {
                    response = await _client.PutAsync($"api/v1.0/values/{key}", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                        "application/json"));
                });

            "Then the response was successful"
                .x(async () =>
                {
                    response.StatusCode.Should().BeEquivalentTo(HttpStatusCode.OK);
                });

            "Then the itemEntity is returned with the right information"
                .x(async () =>
                {
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ValueItemResponse>(json);

                    result.Should().BeEquivalentTo(request);
                });
        }
    }
}       
    