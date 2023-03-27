using SmartVillages.Shared.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVillages.Shared.MarketplaceModels
{
    public class CartItem
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public float Price { get; set; }
        public float Quantity { get; set; }
        public int StatusCode { get; set; }
        public int OrderId { get; set; }
    }
}
