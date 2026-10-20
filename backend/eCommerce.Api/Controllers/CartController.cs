using eCommerce.Api.Contracts.Cart;
using eCommerce.Application.Features.Cart.Commands.AddCartItem;
using eCommerce.Application.Features.Cart.Commands.ClearCart;
using eCommerce.Application.Features.Cart.Commands.RemoveCartItem;
using eCommerce.Application.Features.Cart.Commands.UpdateCartItemQuantity;
using eCommerce.Application.Features.Cart.DTOs;
using eCommerce.Application.Features.Cart.Queries.GetCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace eCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController(
        IMediator mediator) 
        : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private string UserId => User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? throw new UnauthorizedAccessException();

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<CartResponse>> GetCart(CancellationToken ct)
        {
            var cart = await _mediator.Send(new GetCartQuery(UserId), ct);
            return Ok(cart);
        }

        [Authorize]
        [HttpPost("items")]
        public async Task<ActionResult<CartResponse>> AddItem([FromBody] AddCartItemRequest request, CancellationToken ct)
        {
            var cart = await _mediator.Send(new AddCartItemCommand(UserId, request.ProductVariantId, request.Quantity), ct);
            return Ok(cart);
        }

        [Authorize]
        [HttpPut("items/{productVariantId:int}")]
        public async Task<ActionResult<CartResponse>> UpdateQuantity(int productVariantId, UpdateCartItemQuantityRequest request, CancellationToken ct)
        {
            var cart = await _mediator.Send(new UpdateCartItemQuantityCommand(UserId, productVariantId, request.Quantity), ct);
            return Ok(cart);
        }

        [Authorize]
        [HttpDelete("items/{productVariantId:int}")]
        public async Task<ActionResult<CartResponse>> RemoveItem(int productVariantId, CancellationToken ct)
        {
            var cart = await _mediator.Send(new RemoveCartItemCommand(UserId, productVariantId), ct);
            return Ok(cart);
        }

        [Authorize]
        [HttpDelete]
        public async Task<ActionResult<CartResponse>> ClearCart(CancellationToken ct)
        {
            var cart = await _mediator.Send(new ClearCartCommand(UserId), ct);
            return Ok(cart);
        }
    }
}
