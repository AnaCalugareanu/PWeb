import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly themeKey = 'theme';

  constructor() {
    if (this.hasLocalStorage()) {
      const savedTheme = localStorage.getItem(this.themeKey);
      if (savedTheme === 'dark') {
        this.setDarkTheme();
      }
    }
  }

  toggleTheme(): void {
    const isDark = document.documentElement.classList.toggle('dark-theme');
    if (this.hasLocalStorage()) {
      localStorage.setItem(this.themeKey, isDark ? 'dark' : 'light');
    }
  }

  setDarkTheme(): void {
    document.documentElement.classList.add('dark-theme');
    if (this.hasLocalStorage()) {
      localStorage.setItem(this.themeKey, 'dark');
    }
  }

  setLightTheme(): void {
    document.documentElement.classList.remove('dark-theme');
    if (this.hasLocalStorage()) {
      localStorage.setItem(this.themeKey, 'light');
    }
  }

  private hasLocalStorage(): boolean {
    return typeof window !== 'undefined' && !!window.localStorage;
  }
}
