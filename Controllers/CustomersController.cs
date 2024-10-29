using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindApplication.Models;

namespace NorthwindAPI.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class CustomersController : ControllerBase
        {
            private readonly ApplicationDbContext _context;

            public CustomersController(ApplicationDbContext context)
            {
                _context = context;
            }
            [HttpGet]
            public async Task<ActionResult<IEnumerable<Customers>>> GetCustomers()
            {
                return await _context.Customers.ToListAsync();
            }
            [HttpGet("{id}")]
            public async Task<ActionResult<Customers>> GetCustomer(string id)
            {
                var customer = await _context.Customers.FindAsync(id);

                if (customer == null)
                {
                    return NotFound();
                }

                return customer;
            }
            [HttpPost]
            public async Task<ActionResult<Customers>> CreateCustomer(Customers customer)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerID }, customer);
            }
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateCustomer(string id, Customers customer)
            {
                if (!id.Equals(customer.CustomerID))
                {
                    return BadRequest("Customer ID mismatch.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.Entry(customer).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(id))
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
            public async Task<IActionResult> DeleteCustomer(string id)
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            private bool CustomerExists(string id)
            {
                return _context.Customers.Any(e => e.CustomerID.Equals(id));
            }
        }
    }
