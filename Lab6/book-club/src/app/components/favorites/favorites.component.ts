import { Component } from '@angular/core';
import { BookService } from '../../shared/bookService';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzRateModule } from 'ng-zorro-antd/rate';
import { NzTableModule } from 'ng-zorro-antd/table';
import { ModalComponent } from '../../core/modal/modal.component';

@Component({
  selector: 'app-favorites',
  standalone: true,
  imports: [
    NzTableModule,
    CommonModule,
    NzRateModule,
    FormsModule,
    NzButtonModule,
  ],
  templateUrl: './favorites.component.html',
  styleUrl: './favorites.component.scss',
})
export class FavoritesComponent {
  constructor(private bookService: BookService) {}
  wishList: any[] = [];
  wishListId: any;

  ngOnInit(): void {
    this.bookService.wishlist$.subscribe((wishList) => {
      this.wishList = wishList;
    });
  }

  handleRemove(data: any) {
    this.bookService.removeFromWishlist(data);
  }
}
