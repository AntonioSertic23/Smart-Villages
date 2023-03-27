using SmartVillages.Shared.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVillages.Shared.MarketplaceModels
{
    public class ProductRate
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Product Product { get; set; }
        public int Rate { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; }
    }
}
