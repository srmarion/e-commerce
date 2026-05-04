using System.Text.Json.Serialization;

namespace MvcWebApplication.Models
{
    public class OrderDetails
    {
        
        public string OrderId { get; set; }

        public string OrderDetailId { get; set; } 
   
        public int ItemId { get; set; }

        public string Category { get; set; }

        public string Name { get; set; }

        public decimal Cost { get; set; }
    }
}
