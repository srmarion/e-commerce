using DatabaseAccess.Data.EntityModels;
using DatabaseAccess.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Models;
using SharedLibrary.DTO.ShoppingCart;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAppAPI.ApiFunctions
{
	public class ShoppingCartFunctions : IShoppingCartFunctions
	{
		private IShoppingCartData _shoppingCartData;
		private IMenuListingData _menuListingData;
		private readonly ILogger<ShoppingCartFunctions> _logger;

		public ShoppingCartFunctions(IShoppingCartData shoppingCartData, IMenuListingData menuListingData, ILogger<ShoppingCartFunctions> logger)
		{
			_shoppingCartData = shoppingCartData;
			_menuListingData = menuListingData;
			_logger = logger;
			_logger.LogDebug("NLog injected into ShoppingCartFunctions");
		}

		public async Task<List<ShoppingCartGetResponseDTO>> GetShoppingCart(ShoppingCartSearchRequestDTO shoppingCartSearchRequestDTO)
		{
			_logger.LogInformation($"GetShoppingCart was called with shoppingCartSearchRequestDTO: {shoppingCartSearchRequestDTO}");

			var shoppingCartSearch = new ShoppingCartSearch()
			{
				UserId = shoppingCartSearchRequestDTO.UserId,
				Category = shoppingCartSearchRequestDTO.Category
			};

			var dbShoppingCartList = await _shoppingCartData.GetShoppingCart(shoppingCartSearch);

			List<ShoppingCartGetResponseDTO> shoppingCartList = new List<ShoppingCartGetResponseDTO>();

			foreach (var item in dbShoppingCartList)
			{
				var shoppingCartGetResponseDTO = new ShoppingCartGetResponseDTO()
				{
					CartId = item.CartId,
					UserId = item.UserId,
					ItemId = item.ItemId,
					Name = item.Name,
					Category = item.Category,
					Cost = item.Cost
				};

				shoppingCartList.Add(shoppingCartGetResponseDTO);
			}

			return shoppingCartList;
		}

		public async Task CreateShoppingCart(ShoppingCartCreateRequestDTO shoppingCartCreateRequestDTO)
		{
			_logger.LogInformation($"CreateShoppingCart was called with shoppingCartCreateRequestDTO: {shoppingCartCreateRequestDTO}");
			

			var menuListing = await _menuListingData.GetMenuListing(shoppingCartCreateRequestDTO.ItemId);
			var shoppingCartDAO = new ShoppingCartDAO()
			{
				CartId = 0,
                UserId = shoppingCartCreateRequestDTO.UserId,
				ItemId = shoppingCartCreateRequestDTO.ItemId,
				Category = menuListing.Category,
				Cost= menuListing.Cost,
				Name = menuListing.Name

				
            };
			await _shoppingCartData.CreateShoppingCart(shoppingCartDAO);
        }

        public async Task RemoveShoppingCartItem(ShoppingCartRemoveRequestDTO shoppingCartRemoveRequestDTO)
		{
			_logger.LogInformation($"RemoveShoppingCartItem was called with shoppingCartRemoveRequestDTO: {shoppingCartRemoveRequestDTO}");
			await _shoppingCartData.RemoveShoppingCartItem(shoppingCartRemoveRequestDTO.CartId);
        }
        public async Task EmptyShoppingCart(ShoppingCartEmptyRequestDTO shoppingCartEmptyRequestDTO)
		{
            _logger.LogInformation($"EmptyShoppingCart was called with shoppingCartRemoveRequestDTO: {shoppingCartEmptyRequestDTO}");
            await _shoppingCartData.EmptyShoppingCart(shoppingCartEmptyRequestDTO.UserId);

        }

    }
}
