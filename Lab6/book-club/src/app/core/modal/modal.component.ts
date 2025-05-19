import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
import { BookService } from '../../shared/bookService';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzRateModule } from 'ng-zorro-antd/rate';

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [
    NzAvatarModule,
    NzCardModule,
    NzIconModule,
    NzModalModule,
    FormsModule,
    CommonModule,
    NzRateModule,
    NzInputModule,
  ],
  templateUrl: './modal.component.html',
  styleUrl: './modal.component.scss',
})
export class ModalComponent implements OnChanges {
  @Input() modalIsVisible = false;
  @Input() book?: any;
  @Input() reviewId: any;

  @Output() modalClosed: EventEmitter<void> = new EventEmitter<void>();

  reviewText = '';
  tag1 = '';
  tag2 = '';
  tag3 = '';
  reviewGrade = 0;
  reviewDate: Date = new Date();
  reviewAuthor = 'Anonymous';
  bookTitle = '';
  bookAuthor = '';

  constructor(private bookService: BookService) {}

  ngOnChanges(changes: SimpleChanges): void {
    // Reset form fields on modal open or when reviewId/book changes
    if (this.reviewId) {
      const review = this.bookService.getReviewById(this.reviewId);
      if (review) {
        this.reviewText = review.reviewText || '';
        this.tag1 = review.tags?.[0] || '';
        this.tag2 = review.tags?.[1] || '';
        this.tag3 = review.tags?.[2] || '';
        this.reviewGrade = review.grade || 0;
        this.reviewDate = review.date ? new Date(review.date) : new Date();
        this.reviewAuthor = review.author || 'Anonymous';
        this.bookTitle = review.bookTitle || '';
        this.bookAuthor = review.bookAuthor || '';
        return;
      }
    }
    if (this.book) {
      this.bookTitle = this.book.title || '';
      this.bookAuthor = this.book.authorName || '';
    } else {
      this.bookTitle = '';
      this.bookAuthor = '';
    }
    // Reset form fields for new review
    this.reviewText = '';
    this.tag1 = '';
    this.tag2 = '';
    this.tag3 = '';
    this.reviewGrade = 0;
    this.reviewDate = new Date();
    this.reviewAuthor = 'Anonymous';
  }

  handleOk(): void {
    if (
      !this.reviewText.trim() ||
      !this.tag1.trim() ||
      !this.tag2.trim() ||
      !this.tag3.trim() ||
      this.reviewGrade < 1
    ) {
      alert('Please fill in all required fields: review, 3 tags, and a grade.');
      return;
    }

    const review = {
      id: this.reviewId ?? Math.floor(Math.random() * 10000),
      bookTitle: this.bookTitle,
      bookAuthor: this.bookAuthor,
      reviewText: this.reviewText,
      tags: [this.tag1, this.tag2, this.tag3],
      grade: this.reviewGrade,
      date: this.reviewDate,
      author: this.reviewAuthor,
    };

    if (this.reviewId) {
      this.bookService.updateReview(review);
    } else {
      this.bookService.addReview(review);
    }
    this.modalIsVisible = false;
    this.modalClosed.emit();
  }

  handleCancel(): void {
    this.modalIsVisible = false;
    this.modalClosed.emit();
  }
}
