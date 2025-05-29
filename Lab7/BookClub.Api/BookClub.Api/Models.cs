namespace BookClub.Api
{

    public record BookDto(int Id, string Title, int AuthorId, bool IsFavorite);
    public record AuthorDto(int Id, string Name);
    public record ReviewDto(int Id, int BookId, string Text, int Rating);
}
