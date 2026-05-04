using DatabaseAccess.Data.EntityModels;
using DatabaseAccess.Data.Interfaces;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Models;
using SharedLibrary.DTO.Order;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppAPI.ApiFunctions
{
	public class OrdersFunctions : IOrdersFunctions
	{
		private IOrderData _orderData;
		private IShoppingCartData _shoppingCartData;
		private readonly ILogger<OrdersFunctions> _logger;

		public OrdersFunctions(IOrderData orderData, ILogger<OrdersFunctions> logger, IShoppingCartData shoppingCartData)
		{
			_orderData = orderData;
			_shoppingCartData = shoppingCartData;
            _logger = logger;
			_logger.LogDebug("NLog injected into OrdersFunctions");
		}


		public async Task<List<OrderSearchResponseDTO>> GetOrders(OrderSearchRequestDTO orderSearchDTO)
		{
			_logger.LogInformation($"GetOrders was called with orderSearchDTO: {orderSearchDTO}");

			// Not good practice to send DTOs to other layers => separation of concerns
			// Convert DTO to data layer object
			var orderSearch = new OrderSearch()
			{
				UserId = orderSearchDTO.UserId,
				BeginOrderDate = orderSearchDTO.BeginOrderDate,
				EndOrderDate = orderSearchDTO.EndOrderDate
			};

			List<OrderDAO> orderDataList = await _orderData.GetOrders(orderSearch);

			List<OrderSearchResponseDTO> dtoList = new List<OrderSearchResponseDTO>();

			// Not a good practice to pass lower level objects when returning web API data - separation of concerns
			// Convert DAO data to DTO data
			foreach (var daoData in orderDataList)
			{
				var dtoObj = new OrderSearchResponseDTO();
				dtoObj.OrderId = daoData.OrderId;
				dtoObj.UserId = daoData.UserId;
				dtoObj.OrderTotal = daoData.OrderTotal;
				dtoObj.OrderDate = daoData.OrderDate;
				dtoList.Add(dtoObj);
			}

			return dtoList;
		}

		public async Task<OrderGetResponseDTO> GetOrderDetails(OrderGetRequestDTO orderGetRequestDTO)
		{
            _logger.LogInformation($"GetOrderDetails was called with orderGetRequestDTO: {orderGetRequestDTO}");
			//this was the issue. i had to swap arguments.
			var orderDAO = await _orderData.GetOrder(orderGetRequestDTO.OrderId, orderGetRequestDTO.UserId);

			OrderGetResponseDTO orderGetResponseDTO = new OrderGetResponseDTO()
			{
				UserId = orderDAO.UserId,
				OrderId = orderDAO.OrderId,
				OrderTotal = orderDAO.OrderTotal,
				OrderDate = orderDAO.OrderDate
            };

			var orderDetailsDAO = await _orderData.GetOrderDetails(orderGetRequestDTO.OrderId);

            foreach (var orderDetail in orderDetailsDAO)
			{
				var dtoObj = new OrderDetailGetResponseDTO()
				{
					OrderId = orderDetail.OrderId,
					OrderDetailId = orderDetail.OrderDetailId,
					ItemId = orderDetail.ItemId,
					Category = orderDetail.Category,
					Name = orderDetail.Name,
					Cost = orderDetail.Cost,
				};
				orderGetResponseDTO.OrderDetails.Add(dtoObj);
            }

			return orderGetResponseDTO;
                
		}

		public async Task CreateOrder(OrderCreateRequestDTO orderCreateRequestDTO)
		{
            _logger.LogInformation($"CreateOrder was called with orderCreateRequestDTO: {orderCreateRequestDTO}");

			var orderDao = new OrderDAO();
			orderDao.UserId = orderCreateRequestDTO.UserId;
			orderDao.OrderId = System.Guid.NewGuid().ToString();
			orderDao.OrderDate = System.DateTime.Now;

			var shoppingCartSearch = new ShoppingCartSearch()
			{
				UserId = orderCreateRequestDTO.UserId,
				Category = string.Empty,
			};
            var shoppingCart = await _shoppingCartData.GetShoppingCart(shoppingCartSearch);

			var orderDetailsList = new List<OrderDetailDAO>();

			var orderTotal = 0m;

            if (shoppingCart.Any())
			{
				foreach (var item in shoppingCart)
				{
					var orderDetail = new OrderDetailDAO() 
					{	
						OrderId = orderDao.OrderId,
						OrderDetailId = System.Guid.NewGuid().ToString(),
						ItemId = item.ItemId,
						Category = item.Category,
						Name = item.Name,
						Cost= item.Cost
						
						
					};

					orderTotal += item.Cost;
					orderDetailsList.Add(orderDetail);

                }
				orderDao.OrderTotal = orderTotal;

            }
			


            await _orderData.CreateOrder(orderDao, orderDetailsList);

			await _shoppingCartData.EmptyShoppingCart(orderCreateRequestDTO.UserId);
        }
	}
}
