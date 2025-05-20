import { Component, OnInit } from '@angular/core';
import { BookService } from '../../shared/bookService';
import { CardComponent } from '../../core/card/card.component';
import { CommonModule } from '@angular/common';
import { NzButtonComponent } from 'ng-zorro-antd/button';
import { Subscription } from 'rxjs';

import { FormsModule } from '@angular/forms';

import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzDividerModule } from 'ng-zorro-antd/divider';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CardComponent,
    CommonModule,
    NzButtonComponent,
    FormsModule,
    NzButtonModule,
    NzInputModule,
    NzIconModule,
    NzDividerModule,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent implements OnInit {
  books: any[] = [];
  loading: boolean = false;
  private allBooks: any[] = [];
  private currentIndex: number = 0;
  wishlist: any[] = [];
  private wishlistSub?: Subscription;
  searchTerm: string = '';

  constructor(private bookService: BookService) {}

  ngOnInit(): void {
    this.loading = true;
    this.wishlistSub = this.bookService.wishlist$.subscribe((wishlist) => {
      this.wishlist = wishlist;
      this.updateBooksWishlistStatus();
    });

    this.getBooks('harry potter');
  }

  getBooks(search: string) {
    this.loading = true;
    this.bookService.getBooks(search).subscribe(
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
        this.updateBooksWishlistStatus();
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
    this.updateBooksWishlistStatus();
  }

  updateBooksWishlistStatus() {
    this.books = this.books.map((book) => ({
      ...book,
      isInWishlist: this.wishlist.some(
        (w) => w.title === book.title && w.authorName === book.authorName
      ),
    }));
  }

  search(searchTerm: string) {
    this.getBooks(searchTerm);
  }

  ngOnDestroy(): void {
    this.wishlistSub?.unsubscribe();
  }
}
