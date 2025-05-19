import { Component, Input } from '@angular/core';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { BookService } from '../../shared/bookService';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NzRateModule } from 'ng-zorro-antd/rate';
import { NzInputModule } from 'ng-zorro-antd/input';
import { ModalComponent } from '../modal/modal.component';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [
    NzAvatarModule,
    NzCardModule,
    NzModalModule,
    FormsModule,
    CommonModule,
    NzRateModule,
    NzInputModule,
    ModalComponent,
    NzIconModule,
  ],
  templateUrl: './card.component.html',
  styleUrl: './card.component.scss',
})
export class CardComponent {
  @Input() book: any;
  modalIsVisible = false;

  constructor(private bookService: BookService) {}

  handleReview() {
    this.modalIsVisible = true;
  }

  handleAddToWishlist() {
    this.bookService.addToWishlist(this.book);
  }
}
