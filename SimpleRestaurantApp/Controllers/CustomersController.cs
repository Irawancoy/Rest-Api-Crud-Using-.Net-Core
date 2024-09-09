using Microsoft.AspNetCore.Mvc;
using SimpleRestaurantApp.Models;
using SimpleRestaurantApp.Data;
using Microsoft.EntityFrameworkCore;

namespace SimpleRestaurantApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : Controller
    {
        // Inisialisasi DbContext untuk berinteraksi dengan database
        private readonly RestaurantDbContext _context;

        public CustomersController(RestaurantDbContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            // Mengambil daftar semua pelanggan dari database termasuk transaksi yang terkait
            return await _context.Customers.Include(t => t.Transactions).ToListAsync();
        }

        // GET: api/Customers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            // Mengambil pelanggan berdasarkan ID dari database, termasuk transaksi yang terkait
            var customer = await _context.Customers.Include(t => t.Transactions).FirstOrDefaultAsync(c => c.Id == id);

            // Mengembalikan NotFound jika pelanggan dengan ID tersebut tidak ditemukan
            if (customer == null)
            {
                return NotFound();
            }

            // Mengembalikan pelanggan yang ditemukan
            return customer;
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            // Menambahkan pelanggan baru ke konteks
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Mengembalikan respons Created dengan informasi pelanggan yang baru dibuat
            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // PUT: api/Customers/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            // Memeriksa apakah ID pelanggan yang diberikan sesuai dengan ID dalam objek pelanggan
            if (id != customer.Id)
            {
                return BadRequest();
            }

            // Menandai entitas pelanggan sebagai Modified untuk menyimpan perubahan
            _context.Entry(customer).State = EntityState.Modified;

            // Menyimpan perubahan ke database
            await _context.SaveChangesAsync();

            // Mengembalikan NoContent setelah berhasil memperbarui data
            return NoContent();
        }

        // DELETE: api/Customers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            // Mencari pelanggan yang akan dihapus berdasarkan ID
            var customer = await _context.Customers.FindAsync(id);

            // Mengembalikan NotFound jika pelanggan dengan ID tersebut tidak ditemukan
            if (customer == null)
            {
                return NotFound();
            }

            // Menghapus pelanggan dari konteks
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            // Mengembalikan NoContent setelah berhasil menghapus data
            return NoContent();
        }
    }
}
