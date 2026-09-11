using eCommerce.Application.Common.Constants;
using eCommerce.Application.Features.Categories.Commands.CreateCategory;
using eCommerce.Application.Features.Categories.Commands.DeleteCategory;
using eCommerce.Application.Features.Categories.Commands.UpdateCategory;
using eCommerce.Application.Features.Categories.DTOs;
using eCommerce.Application.Features.Categories.Queries.GetAllCategories;
using eCommerce.Application.Features.Categories.Queries.GetCategoryById;
using eCommerce.Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PagedResult<CategoryResponse>>> GetAllCategories([FromQuery]PaginationParams pageParams)
        {
            var categories = await _mediator.Send(new GetAllCategoriesQuery(pageParams));
            return Ok(categories);
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponse?>> GetCategoryToEdit(int id)
        {
            var category = await _mediator.Send(new GetCategoryToEditQuery(id));
            return Ok(category);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<ActionResult<CategoryResponse>> CreateCategory([FromBody]CreateCategoryRequest request)
        {
            var category = await _mediator.Send(new CreateCategoryCommand(request.Name));
            return CreatedAtAction(nameof(GetCategoryToEdit), new { id = category.Id }, category);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryRequest request)
        {
            await _mediator.Send(new UpdateCategoryCommand(id, request.Name));
            return NoContent();
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _mediator.Send(new DeleteCategoryCommand(id));
            return NoContent();
        }
    }
}
