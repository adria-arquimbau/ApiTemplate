using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using ApiTemplate.Values.Api;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xbehave;
using Xunit;
using ApiTemplate.Values.Domain.Entities;
using Voxel.Procurement.Values.Api.Tests;

namespace Procurement.Values.Api.Tests
{
    public class ValuesControllerShould :
        IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient client;
        private readonly CustomWebApplicationFactory<Startup> factory;

        public ValuesControllerShould(
            CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Scenario, AutoData]
        public void GetValuesWithKey(string key, int value)
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotImplemented);

            "Given an item is in the database"
                .x(async () =>
                {
                    var item = new ValueItem(key, value);
                    await factory.RespawnDbContext();
                    await factory.ExecuteDbContextAsync(async context =>
                    {
                        await context.ValueItems.AddAsync(item);
                        await context.SaveChangesAsync();
                    });
                });

            "When we ask for that item through the API"
                .x(async () =>
                {
                    response = await client.GetAsync($"api/v1.0/values/{key}");
                });

            "Then the item is returned with the right information"
                .x(async () =>
                {
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ValueItem>(json);

                    result.Key.Should().Be(key);
                    result.Value.Should().Be(value);
                });
        }

        [Scenario, AutoData]
        public void ReturnAllItems(List<ValueItem> valueItems)
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotImplemented);

            "Given there are items in the database"
                .x(async () =>
                {
                    await factory.RespawnDbContext();
                    await factory.ExecuteDbContextAsync(async context =>
                    {
                        await context.ValueItems.AddRangeAsync(valueItems);
                        await context.SaveChangesAsync();
                    });
                });

            "When we ask for that item through the API"
                .x(async () =>
                {
                    response = await client.GetAsync($"api/v1.0/values/");
                });

            "Then all the items are returned with the right informaiton"
                .x(async () =>
                {
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<ValueItem>>(json);

                    result.Should().BeEquivalentTo(valueItems);
                });
        }

        [Scenario, AutoData]
        public void ReturnNotFoundIfTheKeyIsNotInTheSystem(string key, int value)
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotImplemented);

            "Given an item is not in the database"
                .x(() => { });

            "When we ask for that item through the API"
                .x(async () =>
                {
                    response = await client.GetAsync($"api/v1.0/values/{key}");
                });

            "Then the API returns NotFound"
                .x(async () =>
                {
                    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
                    
                    var result = await response.Content.ReadAsStringAsync();
                    result.Should().Be(key);
                 });
        }

        [Scenario, AutoData]
        public void DeleteASpecificValueItem(string key, int value)
        {
            var deletedResponse = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            var getResponse = new HttpResponseMessage(HttpStatusCode.NotImplemented);

            "Given an item is in the database"
                .x(async () =>
                {
                    var item = new ValueItem(key, value);
                    await factory.RespawnDbContext();
                    await factory.ExecuteDbContextAsync(async context =>
                    {
                        await context.ValueItems.AddAsync(item);
                        await context.SaveChangesAsync();
                    });
                });

            "When we ask for delete the specific item"
                .x(async () =>
                {
                    deletedResponse = await client.DeleteAsync($"api/v1.0/values/{key}");
                });

            "Then the request was successful"
                .x(async () =>
                {
                    deletedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
                });

            "Then the value item was deleted when we ask for the specific deleted item"   
                .x(async () =>
                {
                    getResponse = await client.GetAsync($"api/v1.0/values/{key}");
                });

            "Then the value item not exists"
                .x(async () =>
                {
                    getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
                });
        }
    }
}
