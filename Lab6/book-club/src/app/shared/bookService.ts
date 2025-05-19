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

  private reviewsSubject = new BehaviorSubject<any[]>(
    this.loadReviewsFromStorage()
  );
  reviews$ = this.reviewsSubject.asObservable();

  constructor(private http: HttpClient) {
    this.reviews$.subscribe((reviews) => {
      localStorage.setItem('bookReviews', JSON.stringify(reviews));
    });
  }

  private loadReviewsFromStorage(): any[] {
    const data = localStorage.getItem('bookReviews');
    return data ? JSON.parse(data) : [];
  }

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

  addReview(review: any): void {
    const currentReviews = this.reviewsSubject.value;
    this.reviewsSubject.next([...currentReviews, review]);
  }

  getReviewById(reviewId: any): any | undefined {
    const currentReviews = this.reviewsSubject.value;
    return currentReviews.find((review: any) => review.id === reviewId);
  }

  updateReview(updatedReview: any): void {
    const currentReviews = this.reviewsSubject.value;
    const index = currentReviews.findIndex(
      (review: any) => review.id === updatedReview.id
    );
    if (index !== -1) {
      const newReviews = [...currentReviews];
      newReviews[index] = updatedReview;
      this.reviewsSubject.next(newReviews);
    }
  }
}
