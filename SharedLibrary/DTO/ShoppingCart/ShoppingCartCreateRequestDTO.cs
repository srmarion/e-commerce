using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedLibrary.DTO.ShoppingCart
{
	public class ShoppingCartCreateRequestDTO
	{
        
        [JsonPropertyName("userid")]
        [Required]
        public string UserId { get; set; }

        [JsonPropertyName("itemid")]
        [Required]
        public int ItemId { get; set; }

      

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
