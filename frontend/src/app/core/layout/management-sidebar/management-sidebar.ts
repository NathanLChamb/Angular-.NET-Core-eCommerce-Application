import { Component, inject } from '@angular/core';
import { AuthService } from '../../auth/auth-service';
import { ProductFilterService } from '../../../features/product/product-filter-service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-management-sidebar',
  imports: [RouterLink],
  templateUrl: './management-sidebar.html',
  styleUrl: './management-sidebar.css',
})
export class ManagementSidebar {
  authService = inject(AuthService);
  filterService = inject(ProductFilterService);
  
  resetSearch() {
    this.filterService.reset();
  }
}
