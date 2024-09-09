using Microsoft.AspNetCore.Mvc;
using SimpleRestaurantApp.Models;
using SimpleRestaurantApp.Data;
using Microsoft.EntityFrameworkCore;

namespace SimpleRestaurantApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : Controller
    {
        // Inisialisasi DbContext untuk berinteraksi dengan database
        private readonly RestaurantDbContext _context;

        public TransactionsController(RestaurantDbContext context)
        {
            _context = context;
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            // Mengambil daftar semua transaksi dari database
            return await _context.Transactions.ToListAsync();
        }

        // GET: api/Transactions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            // Mengambil transaksi berdasarkan ID dari database
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);

            // Mengembalikan NotFound jika transaksi dengan ID tersebut tidak ditemukan
            if (transaction == null)
            {
                return NotFound();
            }

            // Mengembalikan transaksi yang ditemukan
            return transaction;
        }

        // POST: api/Transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {
            // Mencari item makanan berdasarkan ID yang diberikan dalam transaksi
            var food = await _context.Foods.FindAsync(transaction.Idfood);

            // Mengembalikan BadRequest jika item makanan tidak ditemukan
            if (food == null)
            {
                return BadRequest("Food item not found.");
            }

            // Menghitung total harga transaksi
            transaction.Totalprice = food.Price * transaction.Quantity;

            // Memeriksa apakah stok cukup untuk transaksi
            if (food.Stock < transaction.Quantity)
            {
                return BadRequest("Insufficient stock.");
            }

            // Mengurangi stok makanan sesuai dengan kuantitas transaksi
            food.Stock -= transaction.Quantity;

            // Mengatur tanggal transaksi ke waktu saat ini
            transaction.Transactiondate = DateTime.Now;

            // Menambahkan transaksi baru ke konteks
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Mengembalikan respons Created dengan informasi transaksi yang baru dibuat
            return CreatedAtAction("GetTransaction", new { id = transaction.Id }, transaction);
        }

        // PUT: api/Transactions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(int id, Transaction transaction)
        {
            // Memeriksa apakah ID transaksi yang diberikan sesuai dengan ID dalam objek transaksi
            if (id != transaction.Id)
            {
                return BadRequest();
            }

            // Mencari transaksi yang ada untuk ID yang diberikan
            var existingTransaction = await _context.Transactions.FindAsync(id);
            if (existingTransaction == null)
            {
                return NotFound();
            }

            // Mencari item makanan berdasarkan ID yang diberikan dalam transaksi yang diperbarui
            var food = await _context.Foods.FindAsync(transaction.Idfood);
            if (food == null)
            {
                return BadRequest("Food item not found.");
            }

            // Mengembalikan stok makanan sesuai dengan kuantitas transaksi yang lama
            food.Stock += existingTransaction.Quantity;

            // Menghitung total harga transaksi yang baru
            transaction.Totalprice = food.Price * transaction.Quantity;

            // Memeriksa apakah stok cukup untuk transaksi yang diperbarui
            if (food.Stock < transaction.Quantity)
            {
                return BadRequest("Insufficient stock.");
            }

            // Mengurangi stok makanan sesuai dengan kuantitas transaksi yang baru
            food.Stock -= transaction.Quantity;

            // Memperbarui informasi transaksi yang ada dengan data dari transaksi yang diperbarui
            existingTransaction.Idcustomers = transaction.Idcustomers;
            existingTransaction.Idfood = transaction.Idfood;
            existingTransaction.Quantity = transaction.Quantity;
            existingTransaction.Totalprice = transaction.Totalprice;
            existingTransaction.Transactiondate = DateTime.Now;

            // Menandai entitas makanan sebagai Modified untuk menyimpan perubahan
            _context.Entry(food).State = EntityState.Modified;

            // Menandai entitas transaksi yang ada sebagai Modified untuk menyimpan perubahan
            _context.Entry(existingTransaction).State = EntityState.Modified;

            // Menyimpan perubahan ke database
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Transactions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            // Mencari transaksi yang akan dihapus berdasarkan ID
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);

            // Mengembalikan NotFound jika transaksi tidak ditemukan
            if (transaction == null)
            {
                return NotFound();
            }

            // Menghapus transaksi dari konteks
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
