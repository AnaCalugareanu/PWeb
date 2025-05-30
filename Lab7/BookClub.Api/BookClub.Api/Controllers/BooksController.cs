using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookClub.Api;
using BookClub.Api.Models;     

namespace BookApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly LibraryContext _db;
    public BooksController(LibraryContext db) => _db = db;

    // ────────────────────────────────────────────────────────────
    // POST /api/books
    // ────────────────────────────────────────────────────────────
    /// <summary>Create a new book.</summary>
    [HttpPost]
    [Authorize(Roles = "ADMIN,WRITER")]
    public async Task<IActionResult> Post([FromBody] BookDto dto)
    {
        if (!ModelState.IsValid)                 
            return BadRequest(ModelState);

        var entity = new Book
        {
            Title = dto.Title,
            AuthorId = dto.AuthorId,
            IsFavorite = dto.IsFavorite
        };

        _db.Books.Add(entity);
        await _db.SaveChangesAsync();

        // build a DTO to return (or map via AutoMapper if you prefer)
        var result = new BookDto(entity.BookId, entity.Title,
                                 entity.AuthorId, entity.IsFavorite);

        // 201 Created + Location header pointing to GET /api/books/{id}
        return CreatedAtAction(nameof(GetById),
                               new { id = entity.BookId },
                               result);
    }

    // ────────────────────────────────────────────────────────────
    // PUT /api/books/{id}
    // ────────────────────────────────────────────────────────────
    /// <summary>Update an existing book.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "ADMIN,WRITER")]
    public async Task<IActionResult> Put(int id, [FromBody] BookDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Route ID and payload ID do not match.");

        var entity = await _db.Books.FindAsync(id);
        if (entity is null)
            return NotFound();

        // apply changes
        entity.Title = dto.Title;
        entity.AuthorId = dto.AuthorId;
        entity.IsFavorite = dto.IsFavorite;

        await _db.SaveChangesAsync();
        return NoContent();                      
    }

    // ────────────────────────────────────────────────────────────
    // DELETE /api/books/{id}
    // ────────────────────────────────────────────────────────────
    /// <summary>Remove a book permanently.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "ADMIN,WRITER")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Books.FindAsync(id);
        if (entity is null)
            return NotFound();

        _db.Books.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();                      
    }


    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Get(int skip = 0, int take = 20)
    {
        var dto = await _db.Books
                           .OrderBy(a => a.BookId)
                           .Skip(skip).Take(take).AsNoTracking().ToListAsync();

        return dto is null ? NotFound() : Ok(dto);
    }

    [AllowAnonymous]                             
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _db.Books.AsNoTracking()
                  .Select(b => new BookDto(b.BookId, b.Title,
                                           b.AuthorId, b.IsFavorite))
                  .FirstOrDefaultAsync(b => b.Id == id);

        return dto is null ? NotFound() : Ok(dto);
    }
}
