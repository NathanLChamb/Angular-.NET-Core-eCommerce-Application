using FluentValidation;

namespace eCommerce.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}
