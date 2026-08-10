using eCommerce.API.Contracts.Orders;
using eCommerce.Application.Common.Constants;
using eCommerce.Application.Features.Orders.Commands.CancelOrder;
using eCommerce.Application.Features.Orders.Commands.CreateOrder;
using eCommerce.Application.Features.Orders.Commands.UpdateOrderStatus;
using eCommerce.Application.Features.Orders.Filters;
using eCommerce.Application.Features.Orders.Queries.GetAdminOrderById;
using eCommerce.Application.Features.Orders.Queries.GetAllOrders;
using eCommerce.Application.Features.Orders.Queries.GetMyOrders;
using eCommerce.Application.Features.Orders.Queries.GetOrderById;
using eCommerce.Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace eCommerce.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private string UserId => User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? throw new UnauthorizedAccessException();
    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateOrderCommand(UserId, request.ShippingAddress), ct);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyOrdersQuery(UserId), ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrderById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id, UserId), ct);
        return Ok(result);
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> CancelOrder(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new CancelOrderCommand(id, UserId), ct);
        return Ok(result);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpGet("admin")]
    public async Task<ActionResult<PagedResult<ReadOrderFromAdminDto>>> GetAllOrders([FromQuery] OrderSearchFilter filter, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllOrdersQuery(filter), ct);
        return Ok(result);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateOrderStatusCommand(id, request.Status), ct);
        return Ok(result);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpGet("admin/{id:int}")]
    public async Task<ActionResult<ReadOrderFromAdminDto>> GetAdminOrderById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAdminOrderByIdQuery(id), ct);
        return Ok(result);
    }

}
