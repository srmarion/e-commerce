using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvcWebApplication.Models;
using MvcWebApplication.ViewModels.Orders;
using SharedLibrary.DTO;
using SharedLibrary.DTO.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MvcWebApplication.ViewFunctions
{
	public class OrdersViewFunctions : IOrdersViewFunctions
	{
		private readonly ILogger<OrdersViewFunctions> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrdersViewFunctions(ILogger<OrdersViewFunctions> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
		{
			_httpClientFactory = httpClientFactory;
			_httpContextAccessor = httpContextAccessor;
			_logger = logger;
			_configuration = configuration;
			_logger.LogDebug("NLog injected into OrdersViewFunctions");
		}

		public async Task GetOrders(IndexViewModel IndexViewModel, HttpContext httpContext)
		{
			_logger.LogInformation($"GetOrders was called with IndexViewModel: {IndexViewModel}");

			// get token from the HttpContext so we can add it to the authorization header
			var token = httpContext.GetTokenAsync("access_token").Result;
			var user = httpContext.User;
			var userId = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

			// Not good practice to pass MVC model to web API - separation of concerns
			// Convert orderSearchViewModel.OrderSearch to OrderSearchDTO
			var orderSearchDto = new OrderSearchRequestDTO()
			{
				UserId = userId,
				BeginOrderDate = IndexViewModel.OrderSearch.BeginOrderDate,
				EndOrderDate = IndexViewModel.OrderSearch.EndOrderDate
			};

			// Serialize the data to be posted
			var jsonSearch = JsonSerializer.Serialize(orderSearchDto);
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

			HttpResponseMessage httpResponse = await client.PostAsync("/api/Orders/GetOrders", data);
			httpResponse.EnsureSuccessStatusCode();
			if (httpResponse.IsSuccessStatusCode)
			{
				response = await httpResponse.Content.ReadAsStringAsync();
			}

			var results = JsonSerializer.Deserialize<List<OrderSearchResponseDTO>>(response);

			// Not good practice to pass DTO into upper layers - separation of concerns
			// Thus need to convert DTO into another class used within a view model
			foreach (var orderDto in results)
			{
				var order = new Order()
				{
					OrderId = orderDto.OrderId,
					UserId = orderDto.UserId,
					OrderDate = orderDto.OrderDate,
					OrderTotal = orderDto.OrderTotal
				};

				IndexViewModel.OrderList.Add(order);
			}

			return;
		}

		public async Task CreateOrder(string userId, HttpContext httpContext)
		{
            _logger.LogInformation($"CreateOrder was called with userId: {userId}");

            // get token from the HttpContext so we can add it to the authorization header
            var token = httpContext.GetTokenAsync("access_token").Result;
            var user = httpContext.User;
			
           

            var orderCreateRequestDTO = new OrderCreateRequestDTO()
			{
				UserId = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value,
            };
            var jsonSearch = JsonSerializer.Serialize(orderCreateRequestDTO);
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

            HttpResponseMessage httpResponse = await client.PostAsync("/api/Orders/CreateOrder", data);
            httpResponse.EnsureSuccessStatusCode();
            if (httpResponse.IsSuccessStatusCode)
            {
                response = await httpResponse.Content.ReadAsStringAsync();
            }


        }

		public async Task GetOrderDetails(string orderId, string userId, GetOrderDetailsViewModel getOrderDetailsViewModel, HttpContext httpContext)
		{
            _logger.LogInformation($"GetOrderDetails was called with orderId: {orderId}");

            // get token from the HttpContext so we can add it to the authorization header
            var token = httpContext.GetTokenAsync("access_token").Result;
            var user = httpContext.User;




            var orderGetRequest = new OrderGetRequestDTO()
            {
             UserId= user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value,
             OrderId = orderId,


            };
            var jsonSearch = JsonSerializer.Serialize(orderGetRequest);
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

            HttpResponseMessage httpResponse = await client.PostAsync("/api/Orders/GetOrder", data);
            httpResponse.EnsureSuccessStatusCode();
            if (httpResponse.IsSuccessStatusCode)
            {
                response = await httpResponse.Content.ReadAsStringAsync();
            }
            var results = JsonSerializer.Deserialize<OrderGetResponseDTO>(response);

			_logger.LogInformation($" see if this worked{results.OrderId}");

			getOrderDetailsViewModel.Order.UserId = results.UserId;
			getOrderDetailsViewModel.Order.OrderId = results.OrderId;
			getOrderDetailsViewModel.Order.OrderDate = results.OrderDate;
			getOrderDetailsViewModel.Order.OrderTotal = results.OrderTotal;


            foreach (var orderDto in results.OrderDetails)
            {
                var order = new OrderDetails()
                {
                    OrderId = orderDto.OrderId,
					OrderDetailId = orderDto.OrderDetailId,
					ItemId = orderDto.ItemId,
					Name = orderDto.Name,
					Category = orderDto.Category,
					Cost = orderDto.Cost
				 };

				getOrderDetailsViewModel.OrderDetailsList.Add(order);

            }

            return;
        }


		public async Task GetUserOrders(UserOrdersViewModel userOrdersViewModel, HttpContext httpContext)
		{
            _logger.LogInformation($"GetUserOrders was called");

            // get token from the HttpContext so we can add it to the authorization header
            var token = httpContext.GetTokenAsync("access_token").Result;
            var user = httpContext.User;






        }
    }
}
