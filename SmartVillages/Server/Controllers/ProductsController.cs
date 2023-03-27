using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using SmartVillages.Server.Data;
using SmartVillages.Shared.MarketplaceModels;

namespace SmartVillages.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<List<ProductViewModel>>> GetProduct()
        {
            List<ProductViewModel> finaleproducts = new List<ProductViewModel>();
            var allproducts = _context.Products.Where(p => p.Deleted != true).Include(f => f.User).Include(f => f.User.UserImage).Include(f => f.User.Place).Include(f => f.ProductCategory).Include(i => i.ProductImage).ToList();

            foreach (var item in allproducts)
            {
                var rates = _context.ProductRate.Where(o => o.Product.Id == item.Id).Include(i => i.User).Include(i => i.User.UserImage).ToList();
                float globalRate = 0;
                if (rates.Count > 0)
                {
                    if (rates.Count > 1)
                    {
                        int count = 0;
                        foreach (var r in rates)
                            count += r.Rate;
                        globalRate = (float)count / (float)rates.Count();
                    }
                    else
                    {
                        globalRate = rates[0].Rate;
                    }
                }
                else
                {
                    globalRate = 0;
                }
                finaleproducts.Add(new ProductViewModel { Id = item.Id, Deleted = item.Deleted, Description = item.Description, Eco = item.Eco, Price = item.Price, ProductCategory = item.ProductCategory, ProductImage = item.ProductImage, ProductRate = globalRate, Quantity = item.Quantity, Title = item.Title, User = item.User });
            }

            return finaleproducts;
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutProduct(Product product)
        {
            var image = await _context.ProductImages.SingleOrDefaultAsync(t => t.Id == product.ProductImage.Id);
            product.ProductImage = image;

            var category = await _context.ProductCategorys.SingleOrDefaultAsync(t => t.Species == product.ProductCategory.Species);
            product.ProductCategory = category;

            var user = await _context.Users.SingleOrDefaultAsync(t => t.Id == product.User.Id);
            product.User = user;

            _context.Entry(product).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            var image = await _context.ProductImages.SingleOrDefaultAsync(t => t.Id == product.ProductImage.Id);
            product.ProductImage = image;

            var category = await _context.ProductCategorys.SingleOrDefaultAsync(t => t.Species == product.ProductCategory.Species);
            product.ProductCategory = category;

            var user = await _context.Users.SingleOrDefaultAsync(t => t.Id == product.User.Id);
            product.User = user;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = _context.Products.Where(p => p.Id == id).FirstOrDefault();
            product.Deleted = true;

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }


        [HttpGet("getlast")]
        public ActionResult<List<ProductViewModel>> GetLast()
        {
            List<ProductViewModel> products = new List<ProductViewModel>();
            var temp = _context.Products.Where(p => p.Deleted != true).OrderByDescending(t => t.Id).Include(f => f.User).Include(f => f.User.UserImage).Include(f => f.User.Place).Include(f => f.ProductCategory).Include(i => i.ProductImage).Take(4).ToList();
            foreach (var t in temp)
            {
                var rates = _context.ProductRate.Where(o => o.Product.Id == t.Id).Include(i => i.User).Include(i => i.User.UserImage).ToList();
                float globalRate = 0;
                if (rates.Count > 0)
                {
                    if (rates.Count > 1)
                    {
                        int count = 0;
                        foreach (var r in rates)
                            count += r.Rate;
                        globalRate = (float)count / (float)rates.Count();
                    }
                    else
                    {
                        globalRate = rates[0].Rate;
                    }
                }
                else
                {
                    globalRate = 0;
                }
                products.Add(new ProductViewModel { Id = t.Id, Deleted = t.Deleted, Description = t.Description, Eco = t.Eco, Price = t.Price, ProductCategory = t.ProductCategory, ProductImage = t.ProductImage, ProductRate = globalRate, Quantity = t.Quantity, Title = t.Title, User = t.User });
            }

            if (products == null)
            {
                return NotFound();
            }
            else
            {
                return products;
            }
        }

        [HttpGet("getmostsold")]
        public ActionResult<List<ProductViewModel>> GetMostSold()
        {
            List<ProductViewModel> products = new List<ProductViewModel>();
            var allproducts = _context.Products.ToList();
            var allcartitems = _context.CartItems.Where(p => p.Product.Deleted != true).Include(f => f.Product.User).Include(f => f.Product.User.UserImage).Include(f => f.Product.User.Place).Include(f => f.Product.ProductCategory).Include(i => i.Product.ProductImage).ToList();
            //var distincted = allcartitems.DistinctBy(b => b.Product.Id).ToList();
            foreach (var item in allproducts)
            {
                var thatitem = allcartitems.Where(p => p.Product.Id == item.Id).FirstOrDefault();
                if (thatitem != null)
                {
                    var sumthatitem = _context.CartItems.Where(p => p.Product.Id == item.Id).Count();
                    if (products.Count < 4)
                    {
                        var rates = _context.ProductRate.Where(o => o.Product.Id == item.Id).Include(i => i.User).Include(i => i.User.UserImage).ToList();
                        float globalRate = 0;
                        if (rates.Count > 0)
                        {
                            if (rates.Count > 1)
                            {
                                int count = 0;
                                foreach (var r in rates)
                                    count += r.Rate;
                                globalRate = (float)count / (float)rates.Count();
                            }
                            else
                            {
                                globalRate = rates[0].Rate;
                            }
                        }
                        else
                        {
                            globalRate = 0;
                        }
                        products.Add(new ProductViewModel { Id = item.Id, Deleted = item.Deleted, Description = item.Description, Eco = item.Eco, Price = item.Price, ProductCategory = item.ProductCategory, ProductImage = item.ProductImage, ProductRate = globalRate, Quantity = item.Quantity, Title = item.Title, User = item.User });
                    }
                    else
                    {
                        foreach (var pro in products.ToList())
                        {
                            var temp = _context.CartItems.Where(p => p.Product.Id == pro.Id).Count();
                            if (temp < sumthatitem)
                            {
                                products.Remove(pro);

                                var rates = _context.ProductRate.Where(o => o.Product.Id == item.Id).Include(i => i.User).Include(i => i.User.UserImage).ToList();
                                float globalRate = 0;
                                if (rates.Count > 0)
                                {
                                    if (rates.Count > 1)
                                    {
                                        int count = 0;
                                        foreach (var r in rates)
                                            count += r.Rate;
                                        globalRate = (float)count / (float)rates.Count();
                                    }
                                    else
                                    {
                                        globalRate = rates[0].Rate;
                                    }
                                }
                                else
                                {
                                    globalRate = 0;
                                }
                                products.Add(new ProductViewModel { Id = item.Id, Deleted = item.Deleted, Description = item.Description, Eco = item.Eco, Price = item.Price, ProductCategory = item.ProductCategory, ProductImage = item.ProductImage, ProductRate = globalRate, Quantity = item.Quantity, Title = item.Title, User = item.User });
                            }
                        }
                    }
                }
            }

            return products;
        }

        [HttpGet("getsearch/{search}")]
        public ActionResult<List<ProductViewModel>> GetSearch(string search)
        {
            List<ProductViewModel> searchedproducts = new List<ProductViewModel>();
            var allproducts = _context.Products.Where(p => p.Title.Contains(search)).Where(p => p.Deleted != true).Include(f => f.User).Include(f => f.User.UserImage).Include(f => f.User.Place).Include(f => f.ProductCategory).Include(i => i.ProductImage).ToList();
            foreach (var item in allproducts)
            {
                var rates = _context.ProductRate.Where(o => o.Product.Id == item.Id).Include(i => i.User).Include(i => i.User.UserImage).ToList();
                float globalRate = 0;
                if (rates.Count > 0)
                {
                    if (rates.Count > 1)
                    {
                        int count = 0;
                        foreach (var r in rates)
                            count += r.Rate;
                        globalRate = (float)count / (float)rates.Count();
                    }
                    else
                    {
                        globalRate = rates[0].Rate;
                    }
                }
                else
                {
                    globalRate = 0;
                }
                searchedproducts.Add(new ProductViewModel { Id = item.Id, Deleted = item.Deleted, Description = item.Description, Eco = item.Eco, Price = item.Price, ProductCategory = item.ProductCategory, ProductImage = item.ProductImage, ProductRate = globalRate, Quantity = item.Quantity, Title = item.Title, User = item.User });
            }

            return searchedproducts;
        }

        [HttpPost("getfilteredproducts")]
        public ActionResult<List<ProductViewModel>> PostProduct(FilterProducts filter)
        {
            List<ProductViewModel> filteredproductvms = new List<ProductViewModel>();
            List<Product> filteredproducts = new List<Product>();

            if (!string.IsNullOrEmpty(filter.Title) || !string.IsNullOrWhiteSpace(filter.Title))
            {
                filteredproducts = _context.Products.Where(p => p.Eco == filter.Eco).Where(p => p.Price >= filter.PriceMin).Where(p => p.Title.Contains(filter.Title)).Where(p => p.Deleted != true).Include(f => f.User).Include(f => f.User.UserImage).Include(f => f.User.Place).Include(f => f.ProductCategory).Include(i => i.ProductImage).ToList();
                if (filter.PriceMax != 0)
                {
                    filteredproducts = filteredproducts.Where(p => p.Price <= filter.PriceMax).ToList();
                }
            }
            else
            {
                filteredproducts = _context.Products.Where(p => p.Eco == filter.Eco).Where(p => p.Price >= filter.PriceMin).Where(p => p.Deleted != true).Include(f => f.User).Include(f => f.User.UserImage).Include(f => f.User.Place).Include(f => f.ProductCategory).Include(i => i.ProductImage).ToList();
                if (filter.PriceMax != 0)
                {
                    filteredproducts = filteredproducts.Where(p => p.Price <= filter.PriceMax).ToList();
                }
            }

            if(!string.IsNullOrEmpty(filter.ProductCategory.Name) && !string.IsNullOrEmpty(filter.ProductCategory.SubCategoryOne) && !string.IsNullOrEmpty(filter.ProductCategory.SubCategoryTwo) && !string.IsNullOrEmpty(filter.ProductCategory.Species))
            {
                filteredproducts = GetFilteredProductCategories(filteredproducts, filter);
            }

            // prebacit product u product view model
            foreach (var item in filteredproducts)
            {
                var rates = _context.ProductRate.Where(o => o.Product.Id == item.Id).Include(i => i.User).Include(i => i.User.UserImage).ToList();
                float globalRate = 0;
                if (rates.Count > 0)
                {
                    if (rates.Count > 1)
                    {
                        int count = 0;
                        foreach (var r in rates)
                            count += r.Rate;
                        globalRate = (float)count / (float)rates.Count();
                    }
                    else
                    {
                        globalRate = rates[0].Rate;
                    }
                }
                else
                {
                    globalRate = 0;
                }
                filteredproductvms.Add(new ProductViewModel { Id = item.Id, Deleted = item.Deleted, Description = item.Description, Eco = item.Eco, Price = item.Price, ProductCategory = item.ProductCategory, ProductImage = item.ProductImage, ProductRate = globalRate, Quantity = item.Quantity, Title = item.Title, User = item.User });
            }

            return filteredproductvms;
        }

        protected List<Product> GetFilteredProductCategories(List<Product> products, FilterProducts filter)
        {
            List<Product> newfilteredlist = new List<Product>();

            newfilteredlist = products.Where(p => p.ProductCategory.Name == filter.ProductCategory.Name).ToList();

            if (filter.ProductCategory.SubCategoryOne != null)
            {
                newfilteredlist = newfilteredlist.Where(p => p.ProductCategory.SubCategoryOne == filter.ProductCategory.SubCategoryOne).ToList();

                if (filter.ProductCategory.SubCategoryTwo != null)
                {
                    newfilteredlist = newfilteredlist.Where(p => p.ProductCategory.SubCategoryTwo == filter.ProductCategory.SubCategoryTwo).ToList();

                    if (filter.ProductCategory.Species != null)
                    {
                        newfilteredlist = newfilteredlist.Where(p => p.ProductCategory.Species == filter.ProductCategory.Species).ToList();
                    }
                }
            }

            return newfilteredlist;
        }
    }
}
