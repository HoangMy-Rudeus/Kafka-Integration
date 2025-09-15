using KafkaMicroservices.InventoryService.Services;
using Microsoft.AspNetCore.Mvc;

namespace KafkaMicroservices.InventoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllInventory()
    {
        try
        {
            var inventory = await _inventoryService.GetAllInventoryAsync();
            return Ok(inventory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inventory");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetInventory(string productId)
    {
        try
        {
            var inventory = await _inventoryService.GetInventoryAsync(productId);
            if (inventory == null)
            {
                return NotFound();
            }
            
            return Ok(inventory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inventory for product {ProductId}", productId);
            return StatusCode(500, "Internal server error");
        }
    }
}
