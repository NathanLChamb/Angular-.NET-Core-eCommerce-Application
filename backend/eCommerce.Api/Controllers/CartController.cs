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

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<CartResponse>> GetCart(CancellationToken ct)
        {
            var cart = await _mediator.Send(new GetCartQuery(), ct);
            return Ok(cart);
        }

        [AllowAnonymous]
        [HttpPost("items")]
        public async Task<ActionResult<CartResponse>> AddItem([FromBody] AddCartItemRequest request, CancellationToken ct)
        {
            var cart = await _mediator.Send(new AddCartItemCommand(request.ProductVariantId, request.Quantity), ct);
            return Ok(cart);
        }

        [AllowAnonymous]
        [HttpPut("items/{productVariantId:int}")]
        public async Task<ActionResult<CartResponse>> UpdateQuantity(int productVariantId, UpdateCartItemQuantityRequest request, CancellationToken ct)
        {
            var cart = await _mediator.Send(new UpdateCartItemQuantityCommand(productVariantId, request.Quantity), ct);
            return Ok(cart);
        }

        [AllowAnonymous]
        [HttpDelete("items/{productVariantId:int}")]
        public async Task<ActionResult<CartResponse>> RemoveItem(int productVariantId, CancellationToken ct)
        {
            var cart = await _mediator.Send(new RemoveCartItemCommand(productVariantId), ct);
            return Ok(cart);
        }

        [AllowAnonymous]
        [HttpDelete]
        public async Task<ActionResult<CartResponse>> ClearCart(CancellationToken ct)
        {
            var cart = await _mediator.Send(new ClearCartCommand(), ct);
            return Ok(cart);
        }
    }
}
