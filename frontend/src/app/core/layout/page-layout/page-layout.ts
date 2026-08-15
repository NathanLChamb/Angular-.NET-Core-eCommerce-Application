import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavBar } from '../nav-bar/nav-bar';
import { Footer } from '../footer/footer';
import { ManagementSidebar } from '../management-sidebar/management-sidebar';
import { CartDrawer } from '../../../features/cart/cart-drawer/cart-drawer';

@Component({
  selector: 'app-page-layout',
  imports: [RouterOutlet, NavBar, Footer, ManagementSidebar, CartDrawer],
  templateUrl: './page-layout.html',
  styleUrl: './page-layout.css',
})
export class PageLayout {

}
