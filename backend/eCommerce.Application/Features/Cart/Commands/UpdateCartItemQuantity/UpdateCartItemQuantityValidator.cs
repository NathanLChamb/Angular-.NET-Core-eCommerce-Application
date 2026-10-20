using FluentValidation;

namespace eCommerce.Application.Features.Cart.Commands.UpdateCartItemQuantity
{
    public class UpdateCartItemQuantityValidator : AbstractValidator<UpdateCartItemQuantityCommand>
    {
        public UpdateCartItemQuantityValidator()
        {
            RuleFor(x => x.ProductVariantId)
                .NotEmpty();

            RuleFor(x => x.Quantity)
                .GreaterThan(0);
        }
    }
}