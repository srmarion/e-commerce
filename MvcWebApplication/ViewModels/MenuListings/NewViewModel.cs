using Microsoft.AspNetCore.Mvc.Rendering;
using MvcWebApplication.Models;
using SharedLibrary.Common.Models;
using System.Collections.Generic;

namespace MvcWebApplication.ViewModels.MenuListings
{
    public class NewViewModel : BaseViewModel
    {
        public MenuListing MenuListing { get; set; } = new MenuListing();
        public List<SelectListItem> CategoryList { get; set; } = new List<SelectListItem>();

    }
}
