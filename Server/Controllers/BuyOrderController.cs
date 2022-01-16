using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
public class BuyOrderController : ControllerBase
{
    private readonly ILogger<BuyOrderController> _logger;

    public BuyOrderController(ILogger<BuyOrderController> logger)
    {
        _logger = logger;
    }

    [HttpPost("buy")]
    public ActionResult MakeBuyOrder([FromForm]BuyRequest buyRequest)
    {
        _logger.LogInformation($"Got buy request: OrderId = {buyRequest.OrderId}, UserId = {buyRequest.UserId}");
        Console.WriteLine($"Got buy request: OrderId = {buyRequest.OrderId}, UserId = {buyRequest.UserId}");
        return Ok();
    }

    [HttpGet("getter")]
    public ActionResult Getter()
    {
        Console.WriteLine($"asdf");
        return Ok("asdf");
    }
}
