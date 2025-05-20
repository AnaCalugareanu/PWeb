import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { ThemeService } from '../../shared/theme.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    NzMenuModule,
    RouterLink,
    RouterLinkActive,
    NzSwitchModule,
    FormsModule,
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  switchValue = false;

  constructor(public themeService: ThemeService) {}

  onThemeToggle() {
    this.themeService.toggleTheme();
  }
}
