using Microsoft.AspNetCore.Mvc.Rendering;
using MvcWebApplication.Models;
using SharedLibrary.Common.Models;
using System.Collections.Generic;

namespace MvcWebApplication.ViewModels.ShoppingCarts
{
    public class SearchViewModel : BaseViewModel
    {
        public SearchViewModel()
        {
            ShoppingCartSearch = new ShoppingCartSearch();
        }

        public List<ShoppingCart> ShoppingCartList { get; set; } = new List<ShoppingCart>();

        public List<SelectListItem> CategoryList { get; set; } = new List<SelectListItem>();

        public ShoppingCartSearch ShoppingCartSearch { get; set; }

        public decimal ShoppingCartTotal { get; set; }
    }
}