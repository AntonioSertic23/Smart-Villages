using SmartVillages.Shared.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVillages.Shared.MarketplaceModels
{
    public class FilterProducts
    {
        public string Title { get; set; }
        public float PriceMin { get; set; }
        public float PriceMax { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public bool Eco { get; set; }
    }
}
