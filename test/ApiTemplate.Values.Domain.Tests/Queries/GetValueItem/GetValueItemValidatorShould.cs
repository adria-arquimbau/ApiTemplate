using FluentAssertions;
using ApiTemplate.Values.Domain.Queries.GetValueItem;
using Xunit;

namespace ApiTemplate.Values.Domain.Tests.Queries.GetValueItem
{
    public class GetValueItemValidatorShould
    {
        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("some string", true)]
        public void ValidateTheRequest(string key, bool isValid)
        {
            var validator = new GetValueItemValidator();
            var request = new GetValueItemRequest(key);

            var result = validator.Validate(request);

            result.IsValid.Should().Be(isValid);
        }
    }
}
