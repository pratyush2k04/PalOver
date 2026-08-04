import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../../shared/sidebar/sidebar';
import { HeaderComponent } from '../../shared/header/header';
import { TopNavComponent } from '../../shared/top-nav/top-nav';
import { ToastComponent } from '../../shared/toast/toast';
// import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, SidebarComponent, ToastComponent,TopNavComponent],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss'
})
export class MainLayoutComponent {}
