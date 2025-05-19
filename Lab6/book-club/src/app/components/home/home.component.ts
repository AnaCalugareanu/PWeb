import { Component } from '@angular/core';
import { BookService } from '../../shared/bookService';
import { CardComponent } from '../../core/card/card.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CardComponent, CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {
  books: any[] = [];

  constructor(private bookService: BookService) {}

  ngOnInit(): void {
    this.bookService.getBooks('angular').subscribe(
      (data) => {
        console.log('Books:', data);
        this.books = data.docs.map((book: any) => ({
          title: book.title,
          authorName: book.author_name
            ? book.author_name.join(', ')
            : 'Unknown',
          coverUrl: book.cover_i
            ? `https://covers.openlibrary.org/b/id/${book.cover_i}-M.jpg`
            : 'https://via.placeholder.com/150',
          publishYear: book.first_publish_year || 'Unknown',
        }));
        this.books = this.books.slice(0, 10); // Limit to 10 books
      },
      (error) => {
        console.error('Error fetching books:', error);
      }
    );
  }
}
