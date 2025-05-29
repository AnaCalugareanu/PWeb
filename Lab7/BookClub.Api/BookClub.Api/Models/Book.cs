namespace BookClub.Api.Models
{
    public class Book {
        public int BookId { get; set; } 
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public bool IsFavorite { get; set; }
    }
}
