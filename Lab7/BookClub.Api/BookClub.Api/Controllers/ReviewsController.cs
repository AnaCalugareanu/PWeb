using BookClub.Api.Models;     
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BookClub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly LibraryContext _db;
        public ReviewsController(LibraryContext db) => _db = db;

        // ────────────────────────────────────────────────
        // GET /api/reviews?skip=0&take=20
        // ────────────────────────────────────────────────
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAll(
            int skip = 0, int take = 20)
            => Ok(await _db.Reviews.AsNoTracking()
                                   .OrderBy(r => r.ReviewId)
                                   .Skip(skip).Take(take)
                                   .Select(r => new ReviewDto(
                                       r.ReviewId, r.BookId, r.Text, r.Rating))
                                   .ToListAsync());

        // ────────────────────────────────────────────────
        // GET /api/reviews/{id}
        // ────────────────────────────────────────────────
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var dto = await _db.Reviews.AsNoTracking()
                             .Where(r => r.ReviewId == id)
                             .Select(r => new ReviewDto(
                                 r.ReviewId, r.BookId, r.Text, r.Rating))
                             .FirstOrDefaultAsync();
            return dto is null ? NotFound() : Ok(dto);
        }

        // ────────────────────────────────────────────────
        // GET /api/reviews/book/7?skip=0&take=20
        // ────────────────────────────────────────────────
        [HttpGet("book/{bookId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetForBook(
            int bookId, int skip = 0, int take = 20)
        {
            bool bookExists = await _db.Books.AnyAsync(b => b.BookId == bookId);
            if (!bookExists) return NotFound($"Book {bookId} not found.");

            var list = await _db.Reviews.AsNoTracking()
                            .Where(r => r.BookId == bookId)
                            .OrderBy(r => r.ReviewId)
                            .Skip(skip).Take(take)
                            .Select(r => new ReviewDto(
                                r.ReviewId, r.BookId, r.Text, r.Rating))
                            .ToListAsync();
            return Ok(list);
        }

        // ────────────────────────────────────────────────
        // POST /api/reviews
        // ────────────────────────────────────────────────
        [HttpPost]
        [Authorize(Roles = "ADMIN,WRITER")]
        public async Task<IActionResult> Post([FromBody] ReviewDto dto)
        {
            if (dto.Rating is < 1 or > 5)
                return BadRequest("Rating must be 1-5.");

            bool bookExists = await _db.Books.AnyAsync(b => b.BookId == dto.BookId);
            if (!bookExists)
                return BadRequest($"Book {dto.BookId} doesn’t exist.");

            var entity = new Review
            {
                BookId = dto.BookId,
                Text = dto.Text,
                Rating = dto.Rating
            };

            _db.Reviews.Add(entity);
            await _db.SaveChangesAsync();

            var result = new ReviewDto(entity.ReviewId, entity.BookId,
                                       entity.Text, entity.Rating);

            return CreatedAtAction(nameof(GetById),
                                   new { id = entity.ReviewId },
                                   result);
        }

        // ────────────────────────────────────────────────
        // PUT /api/reviews/{id}
        // ────────────────────────────────────────────────
        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMIN,WRITER")]
        public async Task<IActionResult> Put(int id, [FromBody] ReviewDto dto)
        {
            if (id != dto.Id) return BadRequest("Route ID ≠ payload ID.");
            if (dto.Rating is < 1 or > 5)
                return BadRequest("Rating must be 1-5.");

            var entity = await _db.Reviews.FindAsync(id);
            if (entity is null) return NotFound();

            entity.Text = dto.Text;
            entity.Rating = dto.Rating;
            await _db.SaveChangesAsync();
            return NoContent();                       
        }

        // ────────────────────────────────────────────────
        // DELETE /api/reviews/{id}
        // ────────────────────────────────────────────────
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN,WRITER")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Reviews.FindAsync(id);
            if (entity is null) return NotFound();

            _db.Reviews.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }

}
