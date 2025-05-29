namespace BookClub.Api.Models
{
    public class Review { 
        public int ReviewId { get; set; }
        public int BookId { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
    }
}
