using eCommerce.Application.Exceptions;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Interfaces.Repositories;
using MediatR;

namespace eCommerce.Application.Features.Options.Commands.DeleteOption
{
    public class DeleteOptionHandler : IRequestHandler<DeleteOptionCommand>
    {
        private readonly IOptionRepository _options;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteOptionHandler(IOptionRepository options, IUnitOfWork unitOfWork)
        {
            _options = options;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteOptionCommand request, CancellationToken ct)
        {
            var option = await _options.GetByIdAsync(request.Id, ct);
            if (option == null) throw new NotFoundException("Option for deletion not found");

            _options.Remove(option);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
