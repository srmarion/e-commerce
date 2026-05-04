using MvcWebApplication.ViewModels.ShoppingCarts;
using System.Threading.Tasks;

namespace MvcWebApplication.ViewFunctions
{
    public interface IShoppingCartViewFunctions
    {
        public Task ProcessIndexRequest(IndexViewModel indexViewModel);
        public Task ProcessEmptyCartRequest();
        public Task ProcessRemoveCartRequest(int cartId);
        public Task ProcessSearchRequest(SearchViewModel searchViewModel);


    }
}
