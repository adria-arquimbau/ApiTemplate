using FluentValidation;

namespace ApiTemplate.Values.Domain.Queries.GetValueItem
{
    public class GetValueItemValidator : AbstractValidator<GetValueItemRequest>
    {
        public GetValueItemValidator()
        {
            RuleFor(x => x.Key).NotNull().NotEmpty();
        }
    }
}
