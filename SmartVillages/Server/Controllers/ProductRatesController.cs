using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartVillages.Server.Data;
using SmartVillages.Shared.MarketplaceModels;

namespace SmartVillages.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductRatesController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductRatesController(DataContext context)
        {
            _context = context;
        }

        // GET: api/ProductRates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductRate>>> GetProductRate()
        {
            return await _context.ProductRate.ToListAsync();
        }

        // GET: api/ProductRates/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductRate>> GetProductRate(int id)
        {
            var productRate = await _context.ProductRate.FindAsync(id);

            if (productRate == null)
            {
                return NotFound();
            }

            return productRate;
        }

        // PUT: api/ProductRates/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutProductRate(ProductRate productRate)
        {
            var user = await _context.Users.SingleOrDefaultAsync(t => t.Id == productRate.User.Id);
            productRate.User = user;
            var product = await _context.Products.SingleOrDefaultAsync(t => t.Id == productRate.Product.Id);
            productRate.Product = product;

            _context.Entry(productRate).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        // POST: api/ProductRates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductRate>> PostProductRate(ProductRate productRate)
        {
            var user = await _context.Users.SingleOrDefaultAsync(t => t.Id == productRate.User.Id);
            productRate.User = user;
            var product = await _context.Products.SingleOrDefaultAsync(t => t.Id == productRate.Product.Id);
            productRate.Product = product;

            _context.ProductRate.Add(productRate);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductRate", new { id = productRate.Id }, productRate);
        }

        // DELETE: api/ProductRates/5
        [HttpDelete("{rateid}")]
        public async Task<IActionResult> DeleteProductRate(int rateid)
        {
            var productRate = await _context.ProductRate.FindAsync(rateid);
            if (productRate == null)
            {
                return NotFound();
            }

            _context.ProductRate.Remove(productRate);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductRateExists(int id)
        {
            return _context.ProductRate.Any(e => e.Id == id);
        }


        [HttpGet("getratesbyuserandorder/{userid}/{productid}")]
        public ActionResult<ProductRate> GetRatesByUserAndOrder(int userid, int productid)
        {
            ProductRate productRate = new ProductRate();
            productRate = _context.ProductRate.Where(p => p.Product.Id == productid).Where(u => u.User.Id == userid).FirstOrDefault();

            return productRate;
        }

        [HttpGet("getlastrates/{productid}")]
        public ActionResult<List<ProductRate>> GetLastRates(int productid)
        {
            List<ProductRate> rates = new List<ProductRate>();
            rates = _context.ProductRate.Where(p => p.Product.Id == productid).Include(i => i.Product).Include(i => i.User).Include(i => i.User.UserImage).Take(3).ToList(); ;
            if (rates == null)
            {
                return NotFound();
            }
            else
            {
                return rates;
            }
        }
    }
}
