import { Component } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { HeaderComponent } from './core/header/header.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [NzButtonModule, RouterOutlet, HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'book-club';

  constructor(private router: Router) {
    if (typeof window !== 'undefined' && window.localStorage) {
      this.router.events.subscribe((event) => {
        if (event instanceof NavigationEnd) {
          localStorage.setItem('lastRoute', event.urlAfterRedirects);
        }
      });
    }
  }

  ngOnInit() {
    if (typeof window !== 'undefined' && window.localStorage) {
      const lastRoute = localStorage.getItem('lastRoute');
      if (lastRoute && lastRoute !== '/' && window.location.pathname === '/') {
        this.router.navigateByUrl(lastRoute);
      }
    }
  }
}
