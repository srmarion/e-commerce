using NuGet.Protocol;

namespace MvcWebApplication.Models
{
    public class ShoppingCart
    {
        public int CartId { get; set; }
        public string UserId { get; set; }
        public int ItemId { get; set; }

        public string Category { get; set; }
        public string Name { get; set; }
        public decimal Cost{ get; set; }

    }
}
