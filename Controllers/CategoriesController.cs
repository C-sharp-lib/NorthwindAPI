using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindApplication.Models;

namespace NorthwindAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController
    {
        private readonly ApplicationDbContext _context;
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("Categories")]
        public async Task<ActionResult<IEnumerable<Categories>>> GetCustomers()
        {
            return await _context.Categories.ToListAsync();
        }
    }
}
