using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartVillages.Shared.UserModels;

namespace SmartVillages.Shared.MarketplaceModels
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Description { get; set; }
        public User Buyer { get; set; }
        public float Price { get; set; }
    }
}
