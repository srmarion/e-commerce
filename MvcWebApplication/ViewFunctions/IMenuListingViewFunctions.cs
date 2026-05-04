using Microsoft.AspNetCore.Http;
using MvcWebApplication.Models;
using MvcWebApplication.ViewModels.MenuListings;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MvcWebApplication.ViewFunctions
{
    public interface IMenuListingViewFunctions
    {
        public Task ProcessIndexRequest(IndexViewModel indexViewModel);
        public Task ProcessSearchRequest(SearchViewModel searchViewModel);
        public Task<int> ProcessCreateRequest(MenuListing menuListing);
        public Task ProcessSaveRequest(MenuListing menuListing);
        public Task ProcessDeleteRequest(MenuListing menuListing);
        public void ProcessInvalidSaveRequest(MenuListing menuListing, EditViewModel editViewModel);
        public void ProcessNewRequest(NewViewModel newViewModel);
        public void ProcessInvalidCreateRequest(MenuListing menuListing, NewViewModel newViewModel);
        
        public Task ProcessEditRequest(int itemId,EditViewModel editViewModel);
        public Task ProcessInvalidDeleteRequest(MenuListing menuListing,EditViewModel editViewModel, Exception ex );




    }
}
