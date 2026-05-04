using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog;
using SharedLibrary.DTO.MenuListing;
using SharedLibrary.DTO.ShoppingCart;
using SharedLibrary.Enums;
using System;
using System.Threading.Tasks;
using WebAppAPI.ApiFunctions;
 
namespace WebAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartsController : Controller
	{
		private IShoppingCartFunctions _shoppingCartFunctions;
        private readonly ILogger<ShoppingCartsController> _logger;

		public ShoppingCartsController(IShoppingCartFunctions shoppingCartFunctions, ILogger<ShoppingCartsController> logger)
		{
			_shoppingCartFunctions = shoppingCartFunctions;
			_logger = logger;
			_logger.LogDebug("NLog injected into ShoppingCartsController");
        }
        [Authorize(Roles = "User")]
        [HttpPost]
		[Route("GetShoppingCart")]
		public async Task<ActionResult> GetShoppingCart(ShoppingCartSearchRequestDTO shoppingCartSearchRequestDTO)
		{
            _logger.LogInformation($"GetShoppingCart was called with shoppingCartSearchRequestDTO: {shoppingCartSearchRequestDTO}");
            try
			{
				var result = await _shoppingCartFunctions.GetShoppingCart(shoppingCartSearchRequestDTO);
                return Ok(result);
            }
			catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred getting menu listings.");
                var responseObject = new { responseText = ex.Message }; // Not necessarily a friendly message
                return StatusCode(StatusCodes.Status500InternalServerError, responseObject);
            }


		}
        [Authorize(Roles = "User")]
        [HttpPost]
		[Route("CreateShoppingCart")]
		public async Task<ActionResult> CreateShoppingCart(ShoppingCartCreateRequestDTO shoppingCartCreateRequestDTO)
		{
            _logger.LogInformation($"CreateShoppingCart was called with shoppingCartCreateRequestDTO: {shoppingCartCreateRequestDTO}");
			try
			{
				await _shoppingCartFunctions.CreateShoppingCart(shoppingCartCreateRequestDTO);
				return Ok(shoppingCartCreateRequestDTO);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred creating shopping cart.");
				var responseObject = new { responseText = ex.Message }; // Not necessarily a friendly message
				return StatusCode(StatusCodes.Status500InternalServerError, responseObject);
            }


            
		}
        [Authorize(Roles = "User")]
        [HttpPost]
		[Route("RemoveShoppingCartItem")]
		public async Task<ActionResult> RemoveShoppingCartItem(ShoppingCartRemoveRequestDTO shoppingCartRemoveRequestDTO)
		{
            _logger.LogInformation($"RemoveShoppingCartItem was called with shoppingCartRemoveRequestDTO: {shoppingCartRemoveRequestDTO}");
            try
            {
				await _shoppingCartFunctions.RemoveShoppingCartItem(shoppingCartRemoveRequestDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred creating shopping cart.");
                var responseObject = new { responseText = ex.Message }; // Not necessarily a friendly message
                return StatusCode(StatusCodes.Status500InternalServerError, responseObject);
            }

        }

		[Authorize(Roles = "User")] // prefer to use enums, but requires custom attribute
		[HttpPost]
		[Route("EmptyShoppingCart")]
		public async Task<ActionResult> EmptyShoppingCart(ShoppingCartEmptyRequestDTO shoppingCartEmptyRequestDTO)
		{
            _logger.LogInformation($"EmptyShoppingCart was called with shoppingCartEmptyRequestDTO: {shoppingCartEmptyRequestDTO}");
            try
            {
                await _shoppingCartFunctions.EmptyShoppingCart(shoppingCartEmptyRequestDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred creating shopping cart.");
                var responseObject = new { responseText = ex.Message }; // Not necessarily a friendly message
                return StatusCode(StatusCodes.Status500InternalServerError, responseObject);
            }
        }
	}
}
