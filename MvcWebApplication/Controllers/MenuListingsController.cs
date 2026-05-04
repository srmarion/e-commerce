using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcWebApplication.ViewModels.MenuListings;
using SharedLibrary.Common.Models;
using SharedLibrary.Enums;
using System.Threading.Tasks;
using System;
using MvcWebApplication.ViewFunctions;
using Microsoft.AspNetCore.Http;
using MvcWebApplication.Models;

namespace MvcWebApplication.Controllers
{
	public class MenuListingsController : Controller
	{
		private readonly ILogger<MenuListingsController> _logger;
		private readonly IMenuListingViewFunctions _menuListingViewFunctions;
        public MenuListingsController(ILogger<MenuListingsController> logger, IMenuListingViewFunctions menuListingViewFunctions)
		{
			_logger = logger;
			_menuListingViewFunctions = menuListingViewFunctions;
			_logger.LogDebug(1, "NLog injected into MenuListingsController");
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Index()
		{
			_logger.LogInformation($"Index was called");
			var indexViewModel = new IndexViewModel();

			try
			{
				await _menuListingViewFunctions.ProcessIndexRequest(indexViewModel);
			}
			catch (Exception ex)
			{
				// Log the exception and return a friendly message back to the client
				_logger.LogError(ex, "Error occurred searching menu listings.");
				indexViewModel.Message = ex.Message;
			}

			return View(indexViewModel);
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Search(MenuListingSearch menuListingSearch)
		{
			_logger.LogInformation($"Search was called with menuListingSearch: {menuListingSearch}");
			var searchViewModel = new SearchViewModel();
			searchViewModel.MenuListingSearch = menuListingSearch; ;

			try
			{
				await _menuListingViewFunctions.ProcessSearchRequest(searchViewModel);
			}
			catch (Exception ex)
			{
				// Log the exception and return a friendly message back to the client
				_logger.LogError(ex, "Error occurred searching menu listings.");
				searchViewModel.Message = ex.Message;
			}

			return View(searchViewModel);
		}
        public async Task<IActionResult> Create(MenuListing menuListing)
        {
            _logger.LogInformation($"Create was called");
			
            if (ModelState.IsValid)
			{

				try
				{
					int id = await _menuListingViewFunctions.ProcessCreateRequest(menuListing);
					return RedirectToAction("Edit", new {itemId = id });

                }
				catch (Exception ex)
				{
					NewViewModel newViewModel = new NewViewModel();
                    _logger.LogError(ex, "Error occurred searching menu listings.");
					newViewModel.Message = ex.Message;
					_menuListingViewFunctions.ProcessNewRequest(newViewModel);
                    newViewModel.MenuListing = menuListing;
					return View("New", newViewModel);


                }
			}
			else
			{
				NewViewModel newViewModel = new NewViewModel();
				try
				{
					 _menuListingViewFunctions.ProcessInvalidCreateRequest(menuListing, newViewModel);
				}
				catch(Exception ex)
				{
					_logger.LogError(ex, "Error occurred Creating menu listings.");
					newViewModel.Message = ex.Message;
                }
            }
            return View();
        }
        public async Task<IActionResult> New()
        {
            _logger.LogInformation($"Index was called");
			var newViewModel = new NewViewModel();


            try
				{
					_menuListingViewFunctions.ProcessNewRequest(newViewModel);


                }
				catch (Exception ex)
				{
                _logger.LogError(ex, "Error occurred searching menu listings.");
				newViewModel.Message = ex.Message;
            }
			
            return View(newViewModel);
        }
        public async Task<IActionResult> Edit(int itemId)
		{
			_logger.LogInformation($"Edit was called");
			var editViewModel = new EditViewModel();
			try
			{
				await _menuListingViewFunctions.ProcessEditRequest(itemId, editViewModel);
				
            }
			catch (Exception ex)
			{
				// Log the exception and return a friendly message back to the client
				_logger.LogError(ex, "Error occurred editing menu listings.");
				editViewModel.Message = ex.Message;
			}
			return View(editViewModel);
        }
		public async Task<IActionResult> Save(MenuListing menuListing)
		{
            _logger.LogInformation($"Save was called");
			if (ModelState.IsValid)
			{
				try
				{
					await _menuListingViewFunctions.ProcessSaveRequest(menuListing);
					return RedirectToAction("Edit", new { itemId = menuListing.ItemId });

				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occurred saving menu listings.");
					var editViewModel = new EditViewModel();
					editViewModel.Message = ex.Message;
					return View("Edit", editViewModel);
                }
			}
			else
			{
				var editViewModel = new EditViewModel();
				try
				{
					_menuListingViewFunctions.ProcessInvalidSaveRequest(menuListing, editViewModel);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occurred saving menu listings.");
					editViewModel.Message = ex.Message;
				}
				return View("Edit", editViewModel);
            }
		}
		public async Task<IActionResult> Delete(MenuListing menuListing)
		{
            _logger.LogInformation($"Delete was called");

            try
            {
                await _menuListingViewFunctions.ProcessDeleteRequest(menuListing);
				return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred deleting menu listings.");
				var editViewModel = new EditViewModel();
                await _menuListingViewFunctions.ProcessInvalidDeleteRequest(menuListing, editViewModel, ex);
				return View("Edit", editViewModel);
            }

            
		}

    }
}
