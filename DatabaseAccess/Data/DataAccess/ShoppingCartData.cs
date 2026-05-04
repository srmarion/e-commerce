using DatabaseAccess.Data.Context;
using DatabaseAccess.Data.EntityModels;
using DatabaseAccess.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Models;
using SharedLibrary.DTO.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Data.DataAccess
{
	public class ShoppingCartData : IShoppingCartData
	{
		private readonly MainAppDbContext _mainAppDbContext;
		private readonly ILogger<ShoppingCartData> _logger;

		public ShoppingCartData(MainAppDbContext mainAppDbContext, ILogger<ShoppingCartData> logger)
		{
			_mainAppDbContext = mainAppDbContext;
			_logger = logger;
			_logger.LogDebug("NLog injected into ShoppingCartData");
		}

		public async Task<List<ShoppingCartDAO>> GetShoppingCart(ShoppingCartSearch shoppingCartSearch)
		{
			_logger.LogInformation($"GetShoppingCart was called with shoppingCartSearch: {shoppingCartSearch}");

			var query = _mainAppDbContext.ShoppingCarts.AsQueryable();

			if (!string.IsNullOrEmpty(shoppingCartSearch.Category))
			{
				query = query.Where(sc => sc.Category == shoppingCartSearch.Category);
			}

			List<ShoppingCartDAO> shoppingCartDAOs = await query.ToListAsync<ShoppingCartDAO>();

			return shoppingCartDAOs;
		}
		public async Task CreateShoppingCart(ShoppingCartDAO shoppingCartDAO)
		{
			_logger.LogInformation($"CreateShoppingCart was called with shoppingCartDAO: {shoppingCartDAO}");
			await _mainAppDbContext.ShoppingCarts.AddAsync(shoppingCartDAO);
			await _mainAppDbContext.SaveChangesAsync();
		}
        public async Task RemoveShoppingCartItem(int shoppingCartRemoveRequestDTO)
		{
            _logger.LogInformation($"RemoveShoppingCartItem was called with shoppingCartRemoveRequestDTO: {shoppingCartRemoveRequestDTO}");

            var query = _mainAppDbContext.ShoppingCarts.AsQueryable();

			var shoppingCartItem = await query.Where(sc => sc.CartId == shoppingCartRemoveRequestDTO).FirstAsync();

            _mainAppDbContext.Remove(shoppingCartItem);

            await _mainAppDbContext.SaveChangesAsync();
        }

        public async Task EmptyShoppingCart(string shoppingCartEmptyRequestDTO)
		{
            _logger.LogInformation($"EmptyShoppingCart was called with shoppingCartEmptyRequestDTO: {shoppingCartEmptyRequestDTO}");

            var query = _mainAppDbContext.ShoppingCarts.AsQueryable();

            var shoppingCartItem = await query.Where(sc => sc.UserId == shoppingCartEmptyRequestDTO).ToListAsync<ShoppingCartDAO>(); 
            _mainAppDbContext.RemoveRange(shoppingCartItem);

            await _mainAppDbContext.SaveChangesAsync();

        }
    }
}
