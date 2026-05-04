using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvcWebApplication.Models;
using MvcWebApplication.ViewModels.Shopping;
using SharedLibrary.DTO.MenuListing;
using SharedLibrary.DTO.ShoppingCart;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MvcWebApplication.ViewFunctions
{
    public class ShoppingViewFunctions : IShoppingViewFunctions
    {
        private readonly ILogger<ShoppingViewFunctions> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShoppingViewFunctions(ILogger<ShoppingViewFunctions> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger.LogDebug("NLog injected into ShoppingViewFunctions");
        }
        public async Task ProcessIndexRequest(IndexViewModel indexViewModel)
        {

            _logger.LogInformation($"ProcessIndexRequest was called with indexViewModel: {indexViewModel}");

            // get token from the HttpContext so we can add it to the authorization header
            var token = _httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
            var user = _httpContextAccessor.HttpContext.User;

            // Not good practice to pass MVC model to web API - separation of concerns
            // Convert orderSearchViewModel.OrderSearch to OrderSearchDTO
            var menuListingSearchDTO = new MenuListingSearchRequestDTO()
            {
                Category = indexViewModel.ShoppingListingSearch.Category
            };
            var jsonSearch = JsonSerializer.Serialize(menuListingSearchDTO);
            var data = new StringContent(jsonSearch, Encoding.UTF8, "application/json");

            var baseAddress = new Uri(_configuration.GetValue<string>("Misc:BaseWebApiUrl"));
            var response = String.Empty; // no ""

            // Create instance of HttpClientFacory
            var client = _httpClientFactory.CreateClient("LocalClient");
            client.BaseAddress = baseAddress;

            // Add authorization header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // To add a cookie to the request instead of using authorization header
            //client.DefaultRequestHeaders.Add("Cookie", $"X-Access-Token={token}");https://localhost:5001/Shopping/AddToCart?itemId=3
            //client.DefaultRequestHeaders.Add("Cookie", $"X-Usernam={user.Identity.Name}");

            HttpResponseMessage httpResponse = await client.PostAsync("/api/MenuListings/GetMenuListings", data);
            httpResponse.EnsureSuccessStatusCode();
            if (httpResponse.IsSuccessStatusCode)
            {
                response = await httpResponse.Content.ReadAsStringAsync();
            }

            var results = JsonSerializer.Deserialize<List<MenuListingGetResponseDTO>>(response);


            foreach (var item in results)
            {
                indexViewModel.MenuListingList.Add(new MenuListing()
                {
                    ItemId = item.ItemId,
                    Name = item.Name,
                    Category = item.Category,
                    Cost = item.Cost
                });
            }

            var menuCategories = Enum.GetNames(typeof(MenuCategories)).ToList();
            foreach (var item in menuCategories)
            {
                indexViewModel.CategoryList.Add(new SelectListItem(item, item));
            }
        }
        
        public async Task ProcessAddToCartException(int itemId, SearchViewModel searchViewModel)
        {
            _logger.LogInformation($"ProcessAddToCartException was called with searchViewModel: {searchViewModel}");
            await ProcessSearchRequest(searchViewModel);
             
        }

        public async Task ProcessAddToCartRequest(int itemId)
        {

            _logger.LogInformation($"ProcessAddToCartRequest was called with itemId: {itemId}");

            // get token from the HttpContext so we can add it to the authorization header
            var token = _httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
            var user = _httpContextAccessor.HttpContext.User;

            // Not good practice to pass MVC model to web API - separation of concerns
            // Convert orderSearchViewModel.OrderSearch to OrderSearchDTO
            var ShoppingCartCreateRequestDTO = new ShoppingCartCreateRequestDTO()
            {
                ItemId = itemId,
                UserId = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value
            };
            var jsonSearch = JsonSerializer.Serialize(ShoppingCartCreateRequestDTO);
            var data = new StringContent(jsonSearch, Encoding.UTF8, "application/json");

            var baseAddress = new Uri(_configuration.GetValue<string>("Misc:BaseWebApiUrl"));
            var response = String.Empty; // no ""

            // Create instance of HttpClientFacory
            var client = _httpClientFactory.CreateClient("LocalClient");
            client.BaseAddress = baseAddress;

            // Add authorization header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // To add a cookie to the request instead of using authorization header
            //client.DefaultRequestHeaders.Add("Cookie", $"X-Access-Token={token}");
            //client.DefaultRequestHeaders.Add("Cookie", $"X-Usernam={user.Identity.Name}");

            HttpResponseMessage httpResponse = await client.PostAsync("/api/ShoppingCarts/CreateShoppingCart", data);
            httpResponse.EnsureSuccessStatusCode();
            if (httpResponse.IsSuccessStatusCode)
            {
                response = await httpResponse.Content.ReadAsStringAsync();
            }
        }

        


        public async Task ProcessSearchRequest(SearchViewModel searchViewModel)
        {
            _logger.LogInformation($"ProcessSearchRequest was called with searchViewModel: {searchViewModel}");

            // get token from the HttpContext so we can add it to the authorization header
            var token = _httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
            var user = _httpContextAccessor.HttpContext.User;

            // Not good practice to pass MVC model to web API - separation of concerns
            // Convert orderSearchViewModel.OrderSearch to OrderSearchDTO
            var menuListingSearchDTO = new MenuListingSearchRequestDTO()
            {
                Category = searchViewModel.ShoppingListingSearch.Category
            };
            var jsonSearch = JsonSerializer.Serialize(menuListingSearchDTO);
            var data = new StringContent(jsonSearch, Encoding.UTF8, "application/json");

            var baseAddress = new Uri(_configuration.GetValue<string>("Misc:BaseWebApiUrl"));
            var response = String.Empty; // no ""

            // Create instance of HttpClientFacory
            var client = _httpClientFactory.CreateClient("LocalClient");
            client.BaseAddress = baseAddress;

            // Add authorization header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // To add a cookie to the request instead of using authorization header
            //client.DefaultRequestHeaders.Add("Cookie", $"X-Access-Token={token}");
            //client.DefaultRequestHeaders.Add("Cookie", $"X-Usernam={user.Identity.Name}");

            HttpResponseMessage httpResponse = await client.PostAsync("/api/MenuListings/GetMenuListings", data);
            httpResponse.EnsureSuccessStatusCode();
            if (httpResponse.IsSuccessStatusCode)
            {
                response = await httpResponse.Content.ReadAsStringAsync();
            }

            var results = JsonSerializer.Deserialize<List<MenuListingGetResponseDTO>>(response);


            foreach (var item in results)
            {
                searchViewModel.MenuListingList.Add(new MenuListing()
                {
                    ItemId = item.ItemId,
                    Name = item.Name,
                    Category = item.Category,
                    Cost = item.Cost
                });
            }

            var menuCategories = Enum.GetNames(typeof(MenuCategories)).ToList();
            foreach (var item in menuCategories)
            {
                searchViewModel.CategoryList.Add(new SelectListItem(item, item));
            }
        }
        
        
    }
}
