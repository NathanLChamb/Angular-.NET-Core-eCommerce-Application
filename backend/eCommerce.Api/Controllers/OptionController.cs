using eCommerce.Application.Common.Constants;
using eCommerce.Application.Features.Options.Commands.CreateOption;
using eCommerce.Application.Features.Options.Commands.DeleteOption;
using eCommerce.Application.Features.Options.Commands.UpdateOption;
using eCommerce.Application.Features.Options.DTOs;
using eCommerce.Application.Features.Options.Queries.GetAllOptions;
using eCommerce.Application.Features.Options.Queries.GetOptionById;
using eCommerce.Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OptionController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OptionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PagedResult<OptionResponse>>> GetAllOptions([FromQuery]PaginationParams pageParams)
        {
            var options = await _mediator.Send(new GetAllOptionsQuery(pageParams));
            return Ok(options);
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<OptionResponse>> GetOptionById(int id)
        {
            var option = await _mediator.Send(new GetOptionByIdQuery(id));
            return Ok(option);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<ActionResult<OptionResponse>> CreateOption([FromBody]CreateOptionRequest dto)
        {
            var option = await _mediator.Send(new CreateOptionCommand(dto.Name, dto.OptionValues));
            return CreatedAtAction(nameof(GetOptionById), new { id = option.Id }, option);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOption(int id, UpdateOptionRequest dto)
        {
            await _mediator.Send(new UpdateOptionCommand(id, dto.Name, dto.OptionValues));
            return NoContent();
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOption(int id)
        {
            await _mediator.Send(new DeleteOptionCommand(id));
            return NoContent();
        }
    }
}
