import { Component, Input } from '@angular/core';

import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { BookService } from '../../shared/bookService';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [NzAvatarModule, NzCardModule, NzIconModule],
  templateUrl: './card.component.html',
  styleUrl: './card.component.scss',
})
export class CardComponent {
  @Input() book: any;

  constructor(private bookService: BookService) {}

  handleReview() {
    const review = {
      bookTitle: this.book.title,
      bookAuthor: this.book.authorName,
      reviewText: 'This is a sample review text.',
    };
    this.bookService.addReview(review);
  }
  handleAddToWishlist() {
    this.bookService.addToWishlist(this.book);
  }
}
