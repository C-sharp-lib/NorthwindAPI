using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindApplication.Models;

namespace NorthwindAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categories>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Categories>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            return category;
        }
        [HttpPost]
        public async Task<ActionResult<Categories>> CreateCategory(Categories category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await _context.Categories.AnyAsync(c => c.CategoryName == category.CategoryName))
            {
                return Conflict($"A category with the name '{category.CategoryName}' already exists.");
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryID }, category);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, Categories category)
        {
            if (id != category.CategoryID)
            {
                return BadRequest("Category ID mismatch.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await _context.Categories.AnyAsync(c => c.CategoryName == category.CategoryName && c.CategoryID != id))
            {
                return Conflict($"Another category with the name '{category.CategoryName}' already exists.");
            }
            _context.Entry(category).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound($"Category with ID {id} not found.");
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryID == id);
        }
    }

}
