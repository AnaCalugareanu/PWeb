import { Component } from '@angular/core';
import { BookService } from '../../shared/bookService';
import { CardComponent } from '../../core/card/card.component';
import { CommonModule } from '@angular/common';
import { NzButtonComponent } from 'ng-zorro-antd/button';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CardComponent, CommonModule, NzButtonComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {
  books: any[] = [];
  loading: boolean = false;
  private allBooks: any[] = [];
  private currentIndex: number = 0;

  constructor(private bookService: BookService) {}

  ngOnInit(): void {
    this.loading = true;
    this.bookService.getBooks('romance').subscribe(
      (data) => {
        this.allBooks = data.docs.map((book: any) => ({
          title: book.title,
          authorName: book.author_name
            ? book.author_name.join(', ')
            : 'Unknown',
          coverUrl: book.cover_i
            ? `https://covers.openlibrary.org/b/id/${book.cover_i}-L.jpg`
            : 'https://media.istockphoto.com/id/1452662817/vector/no-picture-available-placeholder-thumbnail-icon-illustration-design.jpg?s=612x612&w=0&k=20&c=bGI_FngX0iexE3EBANPw9nbXkrJJA4-dcEJhCrP8qMw=',
          publishYear: book.first_publish_year || 'Unknown',
        }));
        this.currentIndex = 12;
        this.books = this.allBooks.slice(0, this.currentIndex);
        this.loading = false;
      },
      (error) => {
        console.error('Error fetching books:', error);
        this.loading = false;
      }
    );
  }

  loadMoreBooks() {
    const nextIndex = this.currentIndex + 12;
    this.books = this.allBooks.slice(0, nextIndex);
    this.currentIndex = nextIndex;
  }
}
