using KafkaMicroservices.InventoryService.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace KafkaMicroservices.InventoryService.Controllers;

/// <summary>
/// API for managing inventory in the microservices demo
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all inventory items
    /// </summary>
    /// <returns>List of inventory items</returns>
    /// <response code="200">Inventory retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Gets inventory for a specific product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <returns>Inventory details for the product</returns>
    /// <response code="200">Inventory found</response>
    /// <response code="404">Product not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{productId}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetInventory([Required] string productId)
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
