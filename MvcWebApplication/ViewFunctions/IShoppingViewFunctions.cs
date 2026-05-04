using MvcWebApplication.ViewModels.Shopping;
using System.Threading.Tasks;

namespace MvcWebApplication.ViewFunctions
{
    public interface IShoppingViewFunctions
    {
        public Task ProcessAddToCartRequest(int itemId);
        public Task ProcessSearchRequest(SearchViewModel searchViewModel);
        
        public Task ProcessIndexRequest(IndexViewModel indexViewModel);
        public Task ProcessAddToCartException(int itemId, SearchViewModel searchViewModel);
    }
}
