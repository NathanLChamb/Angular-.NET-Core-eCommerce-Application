using FluentValidation;

namespace eCommerce.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}
