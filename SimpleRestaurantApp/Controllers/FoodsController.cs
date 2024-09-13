using Microsoft.AspNetCore.Mvc;
using SimpleRestaurantApp.Models;
using SimpleRestaurantApp.Data;
using Microsoft.EntityFrameworkCore;

namespace SimpleRestaurantApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodsController : Controller
    {
        // Inisialisasi DbContext untuk berinteraksi dengan database
        private readonly RestaurantDbContext _context;

        public FoodsController(RestaurantDbContext context)
        {
            _context = context;
        }

        // GET: api/Foods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Food>>> GetFoods()
        {
            // Mengambil daftar semua makanan dari database termasuk transaksi yang terkait
            return await _context.Foods.Include(t=>t.Transactions).ToListAsync();
        }

        // GET: api/Foods/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Food>> GetFood(int id)
        {
            // Mengambil makanan berdasarkan ID dari database
            var food = await _context.Foods.FirstOrDefaultAsync(f => f.Id == id);

            // Mengembalikan NotFound jika makanan dengan ID tersebut tidak ditemukan
            if (food == null)
            {
                return NotFound();
            }

            // Mengembalikan makanan yang ditemukan
            return food;
        }

        // POST: api/Foods
        [HttpPost]
        public async Task<ActionResult<Food>> PostFood(Food food)
        {
            // Menambahkan makanan baru ke konteks
            _context.Foods.Add(food);
            await _context.SaveChangesAsync();

            // Mengembalikan respons Created dengan informasi makanan yang baru dibuat
            return CreatedAtAction("GetFood", new { id = food.Id }, food);
        }

        // PUT: api/Foods/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFood(int id, Food food)
        {
            // Memeriksa apakah ID makanan yang diberikan sesuai dengan ID dalam objek makanan
            if (id != food.Id)
            {
                return BadRequest("ID Makanan tidak cocok dengan ID dalam objek makanan.");
            }

            //Mencari entitas makanan yang ada di database
            var existingFood = await _context.Foods.FindAsync(id);
            if (existingFood == null)
            {
                return NotFound("Makanan tidak ditemukan.");
            }



            // Memperbarui properti makanan yang ada
            existingFood.Name = food.Name;
            existingFood.Price = food.Price;
            existingFood.Stock = food.Stock;

            _context.Entry(existingFood).State = EntityState.Modified;

            try
            {
                // Menyimpan perubahan ke database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Menangani kesalahan concurency
                if (!FoodExists(id))
                {
                    return NotFound("Makanan tidak ditemukan.");
                }
                else
                {
                    throw;
                }
            }

            var result = await _context.Foods.Include(t => t.Transactions).FirstOrDefaultAsync(f => f.Id == id);

            // Mengembalikan data makanan yang telah diperbarui
            return Ok(result);
           


        }

        // DELETE: api/Foods/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFood(int id)
        {
            // Mencari makanan yang akan dihapus berdasarkan ID
            var food = await _context.Foods.FindAsync(id);
            if (food == null)
            {
                return NotFound("Makanan dengan ID tersebut tidak ditemukan.");
            }

            try
            {
            // Menghapus makanan dari konteks
            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();

            }
            catch(Exception ex)
            {
                // Menangani kesalahan jika terjadi saat menghapus
                return StatusCode(StatusCodes.Status500InternalServerError, $"Terjadi kesalahan saat menghapus makanan: {ex.Message}");
            }

            return Ok(new {message = $"Makanan dengan ID {id} telah dihapus."});


        }

        private bool FoodExists(int id)
        {
            return _context.Foods.Any(e => e.Id == id);
        }
    }
}
