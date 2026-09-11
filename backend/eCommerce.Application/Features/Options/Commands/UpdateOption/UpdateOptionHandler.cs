using eCommerce.Application.Exceptions;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Catalog.Options;
using MediatR;

namespace eCommerce.Application.Features.Options.Commands.UpdateOption
{
    public class UpdateOptionHandler : IRequestHandler<UpdateOptionCommand>
    {
        private readonly IOptionRepository _options;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateOptionHandler(IOptionRepository options, IUnitOfWork unitOfWork)
        {
            _options = options;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateOptionCommand request, CancellationToken ct)
        {
            var option = await _options.GetByIdAsync(request.Id, ct);
            if (option == null) throw new NotFoundException("Option for update not found");

            if (request.Name != null) option.Rename(request.Name);

            var requestedIds = request.OptionValues
                .Where(ov => ov.Id.HasValue)
                .Select(ov => ov.Id!.Value)
                .ToHashSet();

            foreach (var existingValue in option.Values.ToList())
            {
                if (!requestedIds.Contains(existingValue.Id))
                    option.RemoveValue(existingValue);
            }

            foreach (var requestedValue in request.OptionValues)
            {
                if (!requestedValue.Id.HasValue)
                {
                    option.AddValue(new OptionValue(requestedValue.Value));
                    continue;
                }

                option.RenameValue(requestedValue.Id.Value, requestedValue.Value);
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
