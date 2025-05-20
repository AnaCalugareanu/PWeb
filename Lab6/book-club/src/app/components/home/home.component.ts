import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
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
import { NzSelectModule } from 'ng-zorro-antd/select';

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
    NzSelectModule,
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
  selectedFilter: string = '';
  selectedSort: string = 'title';

  constructor(
    private bookService: BookService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loading = true;
    this.wishlistSub = this.bookService.wishlist$.subscribe((wishlist) => {
      this.wishlist = wishlist;
      this.updateBooksWishlistStatus();
    });

    this.getBooks('harry potter');
  }
  onFilterChange(event: string) {
    this.selectedFilter = event;
    this.getBooks(this.selectedFilter);
  }
  onSortChange(selectedSort: string) {
    this.selectedSort = selectedSort;
    if (selectedSort === 'title') {
      this.books = [...this.books].sort((a, b) =>
        a.title.localeCompare(b.title)
      );
    } else if (selectedSort === 'author') {
      this.books = [...this.books].sort((a, b) =>
        a.authorName.localeCompare(b.authorName)
      );
    } else if (selectedSort === 'oldest') {
      this.books = [...this.books].sort(
        (a, b) => a.publishYear - b.publishYear
      );
    } else if (selectedSort === 'newest') {
      this.books = [...this.books].sort(
        (a, b) => b.publishYear - a.publishYear
      );
    }
    this.cdr.detectChanges();
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
