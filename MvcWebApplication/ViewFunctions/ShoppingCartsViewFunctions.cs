using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvcWebApplication.Models;
using MvcWebApplication.ViewModels.ShoppingCarts;
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
    public class ShoppingCartsViewFunctions : IShoppingCartViewFunctions
    {
        private readonly ILogger<ShoppingCartsViewFunctions> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShoppingCartsViewFunctions(ILogger<ShoppingCartsViewFunctions> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger.LogDebug("NLog injected into ShoppingCartsViewFunctions");
        }

        public async Task ProcessEmptyCartRequest()
        {
            _logger.LogInformation($"ProcessEmptyCartRequest was called");

            // get token from the HttpContext so we can add it to the authorization header
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            var user = _httpContextAccessor.HttpContext.User;

            var shoppingCartEmptyRequestDTO = new ShoppingCartEmptyRequestDTO() { 

                UserId = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value
            };
            var jsonSearch = JsonSerializer.Serialize(shoppingCartEmptyRequestDTO);
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

            HttpResponseMessage httpResponse = await client.PostAsync("/api/ShoppingCarts/EmptyShoppingCart", data);
            httpResponse.EnsureSuccessStatusCode();
            if (httpResponse.IsSuccessStatusCode)
            {
                response = await httpResponse.Content.ReadAsStringAsync();
            }

            

        }

        public async Task ProcessIndexRequest(IndexViewModel indexViewModel)
        {
            _logger.LogInformation($"ProcessIndexRequest was called with indexViewModel: {indexViewModel}");

            // get token from the HttpContext so we can add it to the authorization header
            var token = _httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
            var user = _httpContextAccessor.HttpContext.User;

            var shoppingCartSearchDTO = new ShoppingCartSearchRequestDTO()
            {
                Category = indexViewModel.ShoppingCartSearch.Category,
                UserId = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value
            };
            var jsonSearch = JsonSerializer.Serialize(shoppingCartSearchDTO);
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

            HttpResponseMessage httpResponse = await client.PostAsync("/api/ShoppingCarts/GetShoppingCart", data);
            httpResponse.EnsureSuccessStatusCode();
            if (httpResponse.IsSuccessStatusCode)
            {
                response = await httpResponse.Content.ReadAsStringAsync();
            }

            var results = JsonSerializer.Deserialize<List<ShoppingCartGetResponseDTO>>(response);

            var total = 0M;

            foreach (var item in results)
            {
                indexViewModel.ShoppingCartList.Add(new ShoppingCart()
                {
                    CartId = item.CartId,
                    UserId = item.UserId,
                    ItemId = item.ItemId,
                    Name = item.Name,
                    Category = item.Category,
                    Cost = item.Cost
                });

                total += item.Cost;
            }

            indexViewModel.ShoppingCartTotal = total;

                var menuCategories = Enum.GetNames(typeof(MenuCategories)).ToList();
            foreach (var item in menuCategories)
            {
                indexViewModel.CategoryList.Add(new SelectListItem(item, item));
            }
        }

        public async Task ProcessRemoveCartRequest(int cartId)
        {
            _logger.LogInformation($"ProcessEmptyCartRequest was called");

            // get token from the HttpContext so we can add it to the authorization header
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            var user = _httpContextAccessor.HttpContext.User;

            var ShoppingCartRemoveRequestDTO = new ShoppingCartRemoveRequestDTO()
            {

               CartId = cartId,
            };
            var jsonSearch = JsonSerializer.Serialize(ShoppingCartRemoveRequestDTO);
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

            HttpResponseMessage httpResponse = await client.PostAsync("/api/ShoppingCarts/RemoveShoppingCartItem", data);
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
            var shoppingCartSearchRequestDTO = new ShoppingCartSearchRequestDTO()
            {
                UserId = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value,
                Category = searchViewModel.ShoppingCartSearch.Category
            };

            // Serialize the data to be posted
            var jsonSearch = JsonSerializer.Serialize(shoppingCartSearchRequestDTO);
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

            HttpResponseMessage httpResponse = await client.PostAsync("/api/ShoppingCarts/GetShoppingCart", data);
            httpResponse.EnsureSuccessStatusCode();
            if (httpResponse.IsSuccessStatusCode)
            {
                response = await httpResponse.Content.ReadAsStringAsync();
            }

            var results = JsonSerializer.Deserialize<List<ShoppingCartGetResponseDTO>>(response);


            foreach (var item in results)
            {
                searchViewModel.ShoppingCartList.Add(new ShoppingCart()
                {
                    CartId = item.CartId,
                    UserId = item.UserId,
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
