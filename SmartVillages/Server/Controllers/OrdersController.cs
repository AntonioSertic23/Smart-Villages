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
    public class OrdersController : ControllerBase
    {
        private readonly DataContext _context;

        public OrdersController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            order.Buyer = _context.Users.Where(p => p.Id == order.Buyer.Id).FirstOrDefault();
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }
        
        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        [HttpGet("getmyorders/{id}")]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetMyOrders(int id)
        {
            List<OrderViewModel> ordersvm = new List<OrderViewModel>();
            var orders = await _context.Orders.Where(o => o.Buyer.Id == id).Include(i => i.Buyer).ToListAsync();
            foreach (var item in orders)
            {
                var products = await _context.CartItems.Where(o => o.OrderId == item.Id).Where(o => o.StatusCode == 1).Include(j => j.Product).Include(j => j.Product.User).ToListAsync();
                if (products.Count == 0)
                    continue;
                ordersvm.Add( new OrderViewModel { Id = item.Id, Buyer = item.Buyer, Description = item.Description, FromDate = item.FromDate, Price = item.Price, CartItems = products, ToDate = item.ToDate });
            }

            return ordersvm;
        }

        [HttpGet("getactiveorders/{id}")]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetActiveOrders(int id)
        {
            List<CartItem> CartItems = await _context.CartItems.Where(o => o.Product.User.Id == id && o.StatusCode == 1).Include(i => i.Product).ToListAsync();
            List<Order> orders = new List<Order>();
            List<OrderViewModel> OrdersVM = new List<OrderViewModel>();

            // trebam orders..
            // za svaki cartitem dohvatti order i stavit u listu pa distinctat..
            foreach (var item in CartItems)
            {
                var order = _context.Orders.Where(o => o.Id == item.OrderId).Include(j => j.Buyer).Include(j => j.Buyer.Place).FirstOrDefault();
                orders.Add(order);
            }

            var Orders = orders.DistinctBy(d => d.Id);

            foreach (var item in Orders)
            {
                var products = await _context.CartItems.Where(o => o.OrderId == item.Id).Include(j => j.Product).Include(j => j.Product.User).ToListAsync();
                OrdersVM.Add(new OrderViewModel { Id = item.Id, Buyer = item.Buyer, Description = item.Description, FromDate = item.FromDate, Price = item.Price, CartItems = products, ToDate = item.ToDate });
            }

            return OrdersVM;
        }

        [HttpGet("getendedorders/{id}")]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetEndedOrders(int id)
        {
            List<CartItem> CartItems = await _context.CartItems.Where(o => o.Product.User.Id == id && o.StatusCode > 1).Include(i => i.Product).ToListAsync();
            List<Order> orders = new List<Order>();
            List<OrderViewModel> OrdersVM = new List<OrderViewModel>();

            // trebam orders..
            // za svaki cartitem dohvatti order i stavit u listu pa distinctat..
            foreach (var item in CartItems)
            {
                var order = _context.Orders.Where(o => o.Id == item.OrderId).Include(j => j.Buyer).Include(j => j.Buyer.Place).FirstOrDefault();
                orders.Add(order);
            }

            var Orders = orders.DistinctBy(d => d.Id);

            foreach (var item in Orders)
            {
                var products = await _context.CartItems.Where(o => o.OrderId == item.Id).Include(j => j.Product).Include(j => j.Product.User).ToListAsync();
                OrdersVM.Add(new OrderViewModel { Id = item.Id, Buyer = item.Buyer, Description = item.Description, FromDate = item.FromDate, Price = item.Price, CartItems = products, ToDate = item.ToDate });
            }

            return OrdersVM;
        }

        
        [HttpPost("setasordered/{id}")]
        public async Task<IActionResult> SetAsOrdered(int id, OrderViewModel order)
        {
            var ord = await _context.CartItems.Where(o => o.OrderId == order.Id).Where(w => w.Product.User.Id == id).ToListAsync();
            foreach (var o in ord)
            {
                o.StatusCode = 2;
                _context.Entry(o).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("getendedorderscustomer/{id}")]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetEndedOrdersCustomer(int id)
        {
            List<OrderViewModel> ordersvm = new List<OrderViewModel>();
            var orders = await _context.Orders.Where(o => o.Buyer.Id == id).Include(i => i.Buyer).Include(i => i.Buyer.Place).ToListAsync();
            foreach (var item in orders)
            {
                var products = await _context.CartItems.Where(o => o.OrderId == item.Id && o.StatusCode > 1).Include(j => j.Product).Include(j => j.Product.User).ToListAsync();
                if (products.Count == 0)
                    continue;
                ordersvm.Add(new OrderViewModel { Id = item.Id, Buyer = item.Buyer, Description = item.Description, FromDate = item.FromDate, Price = item.Price, CartItems = products, ToDate = item.ToDate });
            }

            return ordersvm;
        }
    }
}
