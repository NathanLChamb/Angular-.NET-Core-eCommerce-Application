using eCommerce.Application.Features.Options.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Application.Mapping;
using eCommerce.Domain.Catalog.Options;
using MediatR;

namespace eCommerce.Application.Features.Options.Commands.CreateOption
{
    public class CreateOptionHandler : IRequestHandler<CreateOptionCommand, OptionResponse>
    {
        private readonly IOptionRepository _options;
        private readonly IUnitOfWork _unitOfWork;
        public CreateOptionHandler(IOptionRepository options, IUnitOfWork unitOfWork)
        {
            _options = options;
            _unitOfWork = unitOfWork;
        }

        public async Task<OptionResponse> Handle(CreateOptionCommand request, CancellationToken ct)
        {
            var option = new Option(request.Name);

            foreach (var value in request.OptionValues)
            {
                option.AddValue(new OptionValue(value));
            }

            await _options.AddAsync(option, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return OptionProjection.MapToResponse(option);
        }
    }
}
