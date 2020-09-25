using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ApiTemplate.Values.Domain.Entities;
using ApiTemplate.Values.Domain.Queries.GetValueItems;
using ApiTemplate.Values.Domain.Repositories;
using Xunit;

namespace ApiTemplate.Values.Domain.Tests.Queries.GetValueItems
{
    public class GetValueItemsHandlerShould
    {
        private GetValueItemsQueryHandler queryHandler;
        private IValueItemRepository itemRepository;

        public GetValueItemsHandlerShould()
        {
            itemRepository = Substitute.For<IValueItemRepository>();
            queryHandler = new GetValueItemsQueryHandler(itemRepository, Substitute.For<ILogger<GetValueItemsQueryHandler>>());
        }

        [Theory, AutoData]
        public async Task ReturnAllValueItems(List<ValueItemEntity> valueItems)
        {
            itemRepository.Get().Returns(valueItems);
            var resultItems = await queryHandler.Handle(new GetValueItemsQueryRequest(), CancellationToken.None);
            resultItems.Items.Should().BeEquivalentTo(valueItems);
        }
    }
}
