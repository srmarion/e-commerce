using DatabaseAccess.Data.EntityModels;
using SharedLibrary.Common.Models;
using SharedLibrary.DTO.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Data.Interfaces
{
	public interface IShoppingCartData
	{
		public Task<List<ShoppingCartDAO>> GetShoppingCart(ShoppingCartSearch shoppingCartSearch);
		public  Task CreateShoppingCart(ShoppingCartDAO shoppingCartDAO);
        public Task RemoveShoppingCartItem(int shoppingCartRemoveRequestDTO);
		public Task EmptyShoppingCart(string shoppingCartEmptyRequestDTO);

    }
}
