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
                return BadRequest("ID pelanggan tidak cocok dengan ID dalam objek pelanggan.");
            }

            // Mencari entitas pelanggan yang ada di database
            var existingCustomer = await _context.Customers.FindAsync(id);
            if (existingCustomer == null)
            {
                return NotFound("Pelanggan tidak ditemukan.");
            }

            // Memperbarui entitas pelanggan dengan data dari objek pelanggan
            existingCustomer.Name = customer.Name; 
            existingCustomer.Email = customer.Email; 
            existingCustomer.Phonenumber = customer.Phonenumber;
                                                     

            // Menandai entitas pelanggan sebagai Modified untuk menyimpan perubahan
            _context.Entry(existingCustomer).State = EntityState.Modified;

            try
            {
                // Menyimpan perubahan ke database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Menangani kesalahan ketika entitas tidak dapat diupdate
                if (!CustomerExists(id))
                {
                    return NotFound("Pelanggan tidak ditemukan saat menyimpan perubahan.");
                }
                else
                {
                    throw; // Melempar kembali kesalahan jika ada kesalahan lainnya
                }
            }
            var result = await _context.Customers.Include(t => t.Transactions).FirstOrDefaultAsync(c => c.Id == id);

            // Mengembalikan data pelanggan yang telah diperbarui
            return Ok(result);
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
                return NotFound("Pelanggan dengan ID tersebut tidak ditemukan.");
            }

            try
            {
                // Menghapus pelanggan dari konteks
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Menangani kesalahan jika terjadi saat menghapus
                return StatusCode(StatusCodes.Status500InternalServerError, $"Terjadi kesalahan saat menghapus pelanggan: {ex.Message}");
            }

            // Mengembalikan pesan konfirmasi bahwa pelanggan telah dihapus
            return Ok(new { message = $"Pelanggan dengan ID {id} telah dihapus." });
        }



        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }

}
