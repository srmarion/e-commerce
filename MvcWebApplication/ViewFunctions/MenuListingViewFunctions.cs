using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvcWebApplication.Models;
using MvcWebApplication.ViewModels.MenuListings;

using SharedLibrary.DTO.MenuListing;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MvcWebApplication.ViewFunctions
{
    public class MenuListingViewFunctions : IMenuListingViewFunctions
    {
        private readonly ILogger<MenuListingViewFunctions> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MenuListingViewFunctions(ILogger<MenuListingViewFunctions> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
            _logger.LogDebug("NLog injected into MenuListingViewFunctions");
            _httpContextAccessor = httpContextAccessor;
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
                Category = indexViewModel.MenuListingSearch.Category
            };

            // Serialize the data to be posted
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
                Category = searchViewModel.MenuListingSearch.Category
            };

            // Serialize the data to be posted
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

        public async Task<int> ProcessCreateRequest(MenuListing menuListing)
        {
            _logger.LogInformation($"ProcessCreateRequest was called with menuListing: {menuListing}");

            // get token from the HttpContext so we can add it to the authorization header
            var token = _httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
            var user = _httpContextAccessor.HttpContext.User;

            

            var menuListingCreateDTO = new MenuListingCreateRequestDTO()
            {
                ItemId= menuListing.ItemId,
                Name = menuListing.Name,
                Category = menuListing.Category,
                Cost = menuListing.Cost
            };
            var jsonSearch = JsonSerializer.Serialize(menuListingCreateDTO);
            var data = new StringContent(jsonSearch, Encoding.UTF8, "application/json");

            var baseAddress = new Uri(_configuration.GetValue<string>("Misc:BaseWebApiUrl"));
            var response = String.Empty; // no ""

            // Create instance of HttpClientFacory
            var client = _httpClientFactory.CreateClient("LocalClient");
            client.BaseAddress = baseAddress;

            // Add authorization header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage httpResponse = await client.PostAsync("/api/MenuListings/CreateMenuListing", data);
            httpResponse.EnsureSuccessStatusCode();
            if (httpResponse.IsSuccessStatusCode)
            {
                response = await httpResponse.Content.ReadAsStringAsync();
            }
            var results = JsonSerializer.Deserialize<MenuListingCreateResponseDTO>(response);

            return results.ItemId;
             
            }

        public void ProcessNewRequest(NewViewModel newViewModel)
        {
            var menuCategories = Enum.GetNames(typeof(MenuCategories)).ToList();
            foreach (var item in menuCategories)
            {
                newViewModel.CategoryList.Add(new SelectListItem(item, item));
            }

            
           
        }
        public void ProcessInvalidCreateRequest(MenuListing menuListing,NewViewModel newViewModel)
        {
            _logger.LogInformation($"ProcessInvalidCreateRequest was called with menuListing: {menuListing}");
            var menuCategories = Enum.GetNames(typeof(MenuCategories)).ToList();
            foreach (var item in menuCategories)
            {
                newViewModel.CategoryList.Add(new SelectListItem(item, item));
            }

            
        }

        public async Task ProcessEditRequest(int id, EditViewModel editViewModel)
        {
            _logger.LogInformation($"ProcessEditRequest was called with editViewModel: {editViewModel}");

            // get token from the HttpContext so we can add it to the authorization header
            var token = _httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
            var user = _httpContextAccessor.HttpContext.User;

            // Not good practice to pass MVC model to web API - separation of concerns
            // Convert orderSearchViewModel.OrderSearch to OrderSearchDTO
            var menuListingGetRequestDTO = new MenuListingGetRequestDTO()
            {
                ItemId = id
            };

            // Serialize the data to be posted
            var jsonSearch = JsonSerializer.Serialize(menuListingGetRequestDTO);
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

            HttpResponseMessage httpResponse = await client.PostAsync("/api/MenuListings/GetMenuListing", data);
            httpResponse.EnsureSuccessStatusCode();
            if (httpResponse.IsSuccessStatusCode)
            {
                response = await httpResponse.Content.ReadAsStringAsync();
            }

            var results = JsonSerializer.Deserialize<MenuListingGetResponseDTO>(response);

            editViewModel.MenuListing.ItemId = results.ItemId;
            editViewModel.MenuListing.Name = results.Name;
            editViewModel.MenuListing.Category = results.Category;
            editViewModel.MenuListing.Cost = results.Cost;


            //reload categories
            var menuCategories = Enum.GetNames(typeof(MenuCategories)).ToList();
            foreach (var item in menuCategories)
            {
                editViewModel.CategoryList.Add(new SelectListItem(item, item));
            }


        }

        public async Task ProcessSaveRequest(MenuListing menuListing)
        {
            _logger.LogInformation($"ProcessSaveRequest was called with menuListing: {menuListing}");

            // get token from the HttpContext so we can add it to the authorization header
            var token = _httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
            var user = _httpContextAccessor.HttpContext.User;

            var menuListingUpdateDTO = new MenuListingUpdateRequestDTO()
            {
                ItemId = menuListing.ItemId,
                Name = menuListing.Name,
                Category = menuListing.Category,
                Cost = menuListing.Cost
            };
            var jsonSearch = JsonSerializer.Serialize(menuListingUpdateDTO);
            var data = new StringContent(jsonSearch, Encoding.UTF8, "application/json");

            var baseAddress = new Uri(_configuration.GetValue<string>("Misc:BaseWebApiUrl"));
            var response = String.Empty; // no ""

            // Create instance of HttpClientFacory
            var client = _httpClientFactory.CreateClient("LocalClient");
            client.BaseAddress = baseAddress;

            // Add authorization header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage httpResponse = await client.PostAsync("/api/MenuListings/UpdateMenuListing", data);
            httpResponse.EnsureSuccessStatusCode();
            if (httpResponse.IsSuccessStatusCode)
            {
                response = await httpResponse.Content.ReadAsStringAsync();
            }
            

        }

        public void ProcessInvalidSaveRequest(MenuListing menuListing,EditViewModel editViewModel)
        {
            _logger.LogInformation($"ProcessInvalidSaveRequest was called with menuListing: {menuListing}");

            var menuCategories = Enum.GetNames(typeof(MenuCategories)).ToList();
            foreach (var item in menuCategories)
            {
                editViewModel.CategoryList.Add(new SelectListItem(item, item));
            }
        }

        public async Task ProcessDeleteRequest(MenuListing menuListing)
        {
            _logger.LogInformation($"ProcessDeleteRequest was called with menuListing: {menuListing}");

            // get token from the HttpContext so we can add it to the authorization header
            var token = _httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
            var user = _httpContextAccessor.HttpContext.User;

            var menuListingDeleteDTO = new MenuListingDeleteRequestDTO()
            {
                ItemId = menuListing.ItemId
            };
            var jsonSearch = JsonSerializer.Serialize(menuListingDeleteDTO);
            var data = new StringContent(jsonSearch, Encoding.UTF8, "application/json");

            var baseAddress = new Uri(_configuration.GetValue<string>("Misc:BaseWebApiUrl"));
            var response = String.Empty; // "" doesn't work here

            // Create instance of HttpClientFacory
            var client = _httpClientFactory.CreateClient("LocalClient");
            client.BaseAddress = baseAddress;

            // Add authorization header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // To add a cookie to the request instead of using authorization header
            //client.DefaultRequestHeaders.Add("Cookie", $"X-Access-Token={token}");
            //client.DefaultRequestHeaders.Add("Cookie", $"X-Usernam={user.Identity.Name}");

            HttpResponseMessage httpResponse = await client.PostAsync("/api/MenuListings/DeleteMenuListing", data);
            httpResponse.EnsureSuccessStatusCode();
            if (httpResponse.IsSuccessStatusCode)
            {
                response = await httpResponse.Content.ReadAsStringAsync();
            }
        }

        public async Task ProcessInvalidDeleteRequest(MenuListing menuListing, EditViewModel editViewModel, Exception ex)
        {
            _logger.LogInformation($"ProcessInvalidDeleteRequest was called with editViewModel: {editViewModel}");

            // get token from the HttpContext so we can add it to the authorization header
            var token = _httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
            var user = _httpContextAccessor.HttpContext.User;

            // Not good practice to pass MVC model to web API - separation of concerns
            // Convert orderSearchViewModel.OrderSearch to OrderSearchDTO
            var menuListingGetRequestDTO = new MenuListingGetRequestDTO()
            {
                ItemId = menuListing.ItemId
            };

            // Serialize the data to be posted
            var jsonSearch = JsonSerializer.Serialize(menuListingGetRequestDTO);
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

            HttpResponseMessage httpResponse = await client.PostAsync("/api/MenuListings/DeleteMenuListing", data);
            httpResponse.EnsureSuccessStatusCode();
            if (httpResponse.IsSuccessStatusCode)
            {
                response = await httpResponse.Content.ReadAsStringAsync();
            }

            var results = JsonSerializer.Deserialize<MenuListingGetResponseDTO>(response);

            editViewModel.MenuListing.ItemId = results.ItemId;
            editViewModel.MenuListing.Name = results.Name;
            editViewModel.MenuListing.Category = results.Category;
            editViewModel.MenuListing.Cost = results.Cost;
            editViewModel.Message = ex.Message;


            //reload categories
            var menuCategories = Enum.GetNames(typeof(MenuCategories)).ToList();
            foreach (var item in menuCategories)
            {
                editViewModel.CategoryList.Add(new SelectListItem(item, item));
            }
        }
    }
    }

