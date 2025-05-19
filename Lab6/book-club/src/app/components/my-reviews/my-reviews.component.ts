import { Component } from '@angular/core';
import { BookService } from '../../shared/bookService';
import { NzTableModule } from 'ng-zorro-antd/table';
import { CommonModule } from '@angular/common';
import { NzRateModule } from 'ng-zorro-antd/rate';
import { FormsModule } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { ModalComponent } from '../../core/modal/modal.component';

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
  ],
  templateUrl: './my-reviews.component.html',
  styleUrl: './my-reviews.component.scss',
})
export class MyReviewsComponent {
  constructor(private bookService: BookService) {}
  reviews: any[] = [];
  modalIsVisible = false;
  reviewId: any;

  ngOnInit(): void {
    this.bookService.reviews$.subscribe((reviews) => {
      console.log('Reviews:', reviews);
      this.reviews = reviews;
    });
  }

  onEdit(id: number) {
    this.reviewId = id;
    console.log('🚀 ~ MyReviewsComponent ~ onEdit ~ reviewId:', this.reviewId);
    this.modalIsVisible = true;
  }

  handleModalClosed() {
    this.modalIsVisible = false;
    this.reviewId = null;
  }
}
