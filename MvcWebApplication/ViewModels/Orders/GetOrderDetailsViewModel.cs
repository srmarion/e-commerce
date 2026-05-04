using MvcWebApplication.Models;
using SharedLibrary.Common.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace MvcWebApplication.ViewModels.Orders
{
	public class GetOrderDetailsViewModel : BaseViewModel
	{
        // need more properties to be defined

        public List<OrderDetails> OrderDetailsList { get; set; } = new List<OrderDetails>();

        public Order Order { get; set; } = new Order();

        public string SearchSource { get; set; }


        // holds value to determine which order search to return (Index, GetOrders)
        
	}
}
