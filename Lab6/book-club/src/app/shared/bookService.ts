import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BookService {
  private apiUrl = 'https://openlibrary.org/search.json';

  private wishlistSubject = new BehaviorSubject<any[]>([]);
  wishlist$ = this.wishlistSubject.asObservable();

  private reviewsSubject = new BehaviorSubject<any[]>([]);
  reviews$ = this.reviewsSubject.asObservable();

  constructor(private http: HttpClient) {}

  getBooks(query: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}?q=${encodeURIComponent(query)}`);
  }

  addToWishlist(book: any): void {
    const currentWishlist = this.wishlistSubject.value;
    if (
      !currentWishlist.find(
        (b) => b.title === book.title && b.authorName === book.authorName
      )
    ) {
      this.wishlistSubject.next([...currentWishlist, book]);
    }
  }

  // Add a review written by the user
  addReview(review: any): void {
    const currentReviews = this.reviewsSubject.value;
    this.reviewsSubject.next([...currentReviews, review]);
  }
}
