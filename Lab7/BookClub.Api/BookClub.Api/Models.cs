namespace BookClub.Api
{

    record BookDto(int Id, string Title, int AuthorId, bool IsFavorite);
    record AuthorDto(int Id, string Name);
    record ReviewDto(int Id, int BookId, string Text, int Rating);
}
