namespace Dastardly.WebApi.Controllers;

using Dastardly.Application.Commands;
using Dastardly.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController, Route("api/orders")]
public sealed class OrdersController(IBackgroundCommandDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
    public IActionResult CreateOrder([FromBody] CreateOrderCommand command)
    {
        dispatcher.Enqueue(command);

        return Created();
    }
}
