using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcWebApplication.ViewFunctions;
using MvcWebApplication.ViewModels.ShoppingCarts;
using SharedLibrary.Common.Models;
using System;
using System.Threading.Tasks;

namespace MvcWebApplication.Controllers
{
	public class ShoppingCartsController : Controller
	{
		private readonly ILogger<ShoppingCartsController> _logger;
		private readonly IShoppingCartViewFunctions _shoppingCartViewFunctions;

		public ShoppingCartsController(ILogger<ShoppingCartsController> logger, IShoppingCartViewFunctions shoppingCartViewFunctions)
		{
			_logger = logger;
			_shoppingCartViewFunctions = shoppingCartViewFunctions;
            _logger.LogDebug(1, "NLog injected into ShoppingController");
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Index()
		{
            _logger.LogInformation($"Index was called");
            var indexViewModel = new IndexViewModel();
			try
			{
				await _shoppingCartViewFunctions.ProcessIndexRequest(indexViewModel);
			}
            catch (Exception ex)
            {
                // Log the exception and return a friendly message back to the client
                _logger.LogError(ex, "Error occurred viewing shopping cart.");
                indexViewModel.Message = ex.Message;
            }
            return View(indexViewModel);
            
		}
        [Authorize(Roles = "User")]
        public async Task<IActionResult> EmptyShoppingCart( ) 
		{
			_logger.LogInformation($"EmptyShoppingCart was called");
			var searchViewModel = new SearchViewModel();
			try
			{
				await _shoppingCartViewFunctions.ProcessEmptyCartRequest();
				return RedirectToAction("Index");
            }
			catch (Exception ex)
			{
				var indexViewModel = new IndexViewModel();
				indexViewModel.Message = ex.Message;
                // Log the exception and return a friendly message back to the client
                _logger.LogError(ex, $"Error occurred removing item from shopping cart.");
				return View("Index", indexViewModel);
			}
        }
        public async Task<IActionResult> Remove( int cartId) 
		{
			_logger.LogInformation($"EmptyShoppingCart was called");
			var searchViewModel = new SearchViewModel();
			try
			{
				await _shoppingCartViewFunctions.ProcessRemoveCartRequest(cartId);
				return RedirectToAction("Index");
            }
			catch (Exception ex)
			{
				var indexViewModel = new IndexViewModel();
				indexViewModel.Message = ex.Message;
                // Log the exception and return a friendly message back to the client
                _logger.LogError(ex, $"Error occurred removing item from shopping cart.");
				return View("Index", indexViewModel);
			}
			
        }
		public async Task<IActionResult> Search(ShoppingCartSearch shoppingCartSearch)
		{
            _logger.LogInformation($"Search was called with shoppingListingSearch: {shoppingCartSearch}");
            var searchViewModel = new SearchViewModel();
			searchViewModel.ShoppingCartSearch = shoppingCartSearch;
            try
            {
                await _shoppingCartViewFunctions.ProcessSearchRequest(searchViewModel);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred searching Shopping cart.");
                //await _shoppingViewFunctions.ProcessSearchException(itemId, searchViewModel);
                searchViewModel.Message = ex.Message;
            }
            return View(searchViewModel);

        }
	}
}
