using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookClub.Api.Models;
using BookClub.Api;

namespace BookApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly LibraryContext _db;
    public AuthorsController(LibraryContext db) => _db = db;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAll(
        int skip = 0, int take = 20)
        => Ok(await _db.Authors.AsNoTracking()
                               .OrderBy(a => a.AuthorId)
                               .Skip(skip).Take(take)
                               .Select(a => new AuthorDto(a.AuthorId, a.Name))
                               .ToListAsync());


    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(
        int id, bool includeBooks = false, int skip = 0, int take = 20)
    {
        var author = await _db.Authors.AsNoTracking()
                                      .FirstOrDefaultAsync(a => a.AuthorId == id);
        if (author is null) return NotFound();

        var dto = new AuthorDto(author.AuthorId, author.Name);

        if (!includeBooks) return Ok(dto);

        var books = await _db.Books.AsNoTracking()
                                   .Where(b => b.AuthorId == id)
                                   .OrderBy(b => b.AuthorId)
                                   .Skip(skip).Take(take)
                                   .Select(b => new BookDto(
                                       b.AuthorId, b.Title, b.AuthorId, b.IsFavorite))
                                   .ToListAsync();

        return Ok(new { Author = dto, Books = books });
    }

    // ───────────────────────────────────────────── POST
    // POST /api/authors
    [HttpPost]
    [Authorize(Roles = "ADMIN,WRITER")]
    public async Task<IActionResult> Post([FromBody] AuthorDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var entity = new Author { Name = dto.Name };
        _db.Authors.Add(entity);
        await _db.SaveChangesAsync();

        var result = new AuthorDto(entity.AuthorId, entity.Name);
        return CreatedAtAction(nameof(GetById), new { id = entity.AuthorId }, result);
    }

    // ───────────────────────────────────────────── PUT
    // PUT /api/authors/5
    [HttpPut("{id:int}")]
    [Authorize(Roles = "ADMIN,WRITER")]
    public async Task<IActionResult> Put(int id, [FromBody] AuthorDto dto)
    {
        if (id != dto.Id) return BadRequest("Route ID and payload ID differ.");

        var entity = await _db.Authors.FindAsync(id);
        if (entity is null) return NotFound();

        entity.Name = dto.Name;
        await _db.SaveChangesAsync();
        return NoContent();   // 204
    }

    // ───────────────────────────────────────────── DELETE
    // DELETE /api/authors/5
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "ADMIN,WRITER")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Authors.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Authors.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();   // 204
    }
}
