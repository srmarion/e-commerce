using DatabaseAccess.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcWebApplication.ViewModels.Shopping;
using SharedLibrary.Enums;
using MvcWebApplication.ViewFunctions;
using System;
using System.Threading.Tasks;
using SharedLibrary.Common.Models;

namespace MvcWebApplication.Controllers
{

	public class ShoppingController : Controller
	{
        private readonly ILogger<ShoppingController> _logger;
        //add a link to a view functions page. either home or order. need to figure this out.
        private readonly IShoppingViewFunctions _shoppingViewFunctions;

        public ShoppingController(ILogger<ShoppingController> logger, IShoppingViewFunctions shoppingViewFunctions)
        {
            _logger = logger;
            _shoppingViewFunctions = shoppingViewFunctions;
            _logger.LogDebug(1, "NLog injected into ShoppingController");
        }

        [Authorize(Roles = "User")]
		public async Task<IActionResult> Index()
		{

            _logger.LogInformation($"Index was called");
            var indexViewModel = new IndexViewModel();
            try
            {
                
                await _shoppingViewFunctions.ProcessIndexRequest(indexViewModel);
            }
            catch (Exception ex)
            {
                // Log the exception and return a friendly message back to the client
                _logger.LogError(ex, "Error occurred searching Shopping listings.");
                indexViewModel.Message = ex.Message;
            }
            return View(indexViewModel);
		}

        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddToCart(int itemId, string category)
        {
            _logger.LogInformation($"AddToCart was called with itemId: {itemId}");
            SearchViewModel searchViewModel = new SearchViewModel();
            searchViewModel.ShoppingListingSearch.Category = category;
            try
            {
                await _shoppingViewFunctions.ProcessAddToCartRequest(itemId); 
                searchViewModel.ShoppingListingSearch.isRedirectedFromAddToCart = true;
                return RedirectToAction("Search", searchViewModel.ShoppingListingSearch);
            }
            catch(Exception ex)
            {
                _logger .LogError($"Error occurred adding to cart.");
                await _shoppingViewFunctions.ProcessAddToCartException(itemId, searchViewModel);
                searchViewModel.Message = ex.Message;
                return View("Search", searchViewModel);
            }
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Search(ShoppingListingSearch shoppingListingSearch)
        {
            _logger.LogInformation($"Search was called with itemId: {shoppingListingSearch}");
            
            var searchViewModel = new SearchViewModel();
            searchViewModel.ShoppingListingSearch = shoppingListingSearch;
            try
            {
                await _shoppingViewFunctions.ProcessSearchRequest(searchViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred searching Shopping listings.");
                //await _shoppingViewFunctions.ProcessSearchException(itemId, searchViewModel);
                searchViewModel.Message = ex.Message;
            }
            return View(searchViewModel);
        }




    }
}
