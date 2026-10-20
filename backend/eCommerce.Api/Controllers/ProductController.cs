using eCommerce.Application.Common.Constants;
using eCommerce.Application.Features.Products.Commands.CreateProduct;
using eCommerce.Application.Features.Products.Commands.DeleteProduct;
using eCommerce.Application.Features.Products.Commands.UpdateProduct;
using eCommerce.Application.Features.Products.DTOs;
using eCommerce.Application.Features.Products.Filters;
using eCommerce.Application.Features.Products.Queries.GetAllProducts;
using eCommerce.Application.Features.Products.Queries.GetProductById;
using eCommerce.Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(
        IMediator mediator) 
        : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductResponse>>> GetAllProducts([FromQuery] ProductSearchFilter filter)
        {
            var products = await _mediator.Send(new GetAllProductsQuery(filter));
            return Ok(products);
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse?>> GetProductById(int id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));
            return Ok(product);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] CreateProductRequest dto)
        {
            var product = await _mediator.Send(new CreateProductCommand(dto.Name, dto.Description, dto.CategoryIds, dto.OptionIds, dto.ProductVariants));
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductRequest dto)
        {
            await _mediator.Send(new UpdateProductCommand(id, dto.Name, dto.Description, dto.CategoryIds, dto.OptionIds, dto.ProductVariants));
            return NoContent();
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _mediator.Send(new DeleteProductCommand(id));
            return NoContent();
        }
    }
}
