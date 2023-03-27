using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVillages.Shared.MarketplaceModels
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }                // Biljka       // Životinja        // Mehanizacija
        public string SubCategoryOne { get; set; }      // Voće         // Govedo           // Traktor
        public string SubCategoryTwo { get; set; }      // Jabuka       // Krava            // -||-
        public string Species { get; set; }             // Jonagold     // Simentalac       // -||-
        public string Brand { get; set; }               // -||-         // -||-             // John Deer
    }
}
