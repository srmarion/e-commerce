
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcWebApplication.Models;
using SharedLibrary.Common.Models;
using System.Collections.Generic;

namespace MvcWebApplication.ViewModels.Shopping
{
    public class IndexViewModel : BaseViewModel
    {
        public IndexViewModel()
        {
            ShoppingListingSearch = new ShoppingListingSearch();
        }

        public List<MenuListing> MenuListingList { get; set; } = new List<MenuListing>();

        public List<SelectListItem> CategoryList { get; set; } = new List<SelectListItem>();

        public ShoppingListingSearch ShoppingListingSearch { get; set; }
    }
}
