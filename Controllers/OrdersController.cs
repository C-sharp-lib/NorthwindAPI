using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindApplication.Models;

namespace NorthwindAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Orders>>> GetOrders()
        {
            return await _context.Orders
                                 .Include(o => o.Customers)
                                 .ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Orders>> GetOrder(int id)
        {
            var order = await _context.Orders
                                      .Include(o => o.Customers)
                                      .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }
        [HttpPost]
        public async Task<ActionResult<Orders>> CreateOrder(Orders order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerID == order.CustomerID);
            if (!customerExists)
            {
                return BadRequest($"Customer with ID {order.CustomerID} does not exist.");
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await _context.Entry(order).Reference(o => o.CustomerID).LoadAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderID }, order);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Orders order)
        {
            if (id != order.OrderID)
            {
                return BadRequest("Order ID mismatch.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerID == order.CustomerID);
            if (!customerExists)
            {
                return BadRequest($"Customer with ID {order.CustomerID} does not exist.");
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
            return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}
