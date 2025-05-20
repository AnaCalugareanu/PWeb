import { Component } from '@angular/core';
import { BookService } from '../../shared/bookService';
import { NzTableModule } from 'ng-zorro-antd/table';
import { CommonModule } from '@angular/common';
import { NzRateModule } from 'ng-zorro-antd/rate';
import { FormsModule } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { ModalComponent } from '../../core/modal/modal.component';
import { NzModalModule } from 'ng-zorro-antd/modal';

@Component({
  selector: 'app-my-reviews',
  standalone: true,
  imports: [
    NzTableModule,
    CommonModule,
    NzRateModule,
    FormsModule,
    NzButtonModule,
    ModalComponent,
    NzModalModule,
  ],
  templateUrl: './my-reviews.component.html',
  styleUrl: './my-reviews.component.scss',
})
export class MyReviewsComponent {
  reviews: any[] = [];
  modalIsVisible = false;
  removeModalIsVisible = false;
  reviewId: any;
  bookTitle = '';
  bookAuthor = '';

  constructor(private bookService: BookService) {}

  ngOnInit(): void {
    this.bookService.reviews$.subscribe((reviews) => {
      this.reviews = reviews;
    });
  }

  onEdit(id: number) {
    this.reviewId = id;
    this.modalIsVisible = true;
  }

  handleModalClosed() {
    this.modalIsVisible = false;
    this.reviewId = null;
  }

  handleCancel() {
    this.removeModalIsVisible = false;
    this.reviewId = null;
  }
  handleOk() {
    this.handleRemoveReview(this.reviewId);
    this.removeModalIsVisible = false;
  }

  openRemoveIdModal(id: number, bookTitle: string, bookAuthor: string) {
    this.reviewId = id;
    this.removeModalIsVisible = true;
    this.bookTitle = bookTitle;
    this.bookAuthor = bookAuthor;
  }

  handleRemoveReview(id: number) {
    this.bookService.removeReview(id);
  }
}
