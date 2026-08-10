import { Component, inject, input } from '@angular/core';
import { OrderService } from '../order-service';
import { rxResource } from '@angular/core/rxjs-interop';
import { EMPTY } from 'rxjs';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-admin-order-details',
  imports: [DatePipe],
  templateUrl: './admin-order-details.html',
  styleUrl: './admin-order-details.css',
})
export class AdminOrderDetails {
  private orderService = inject(OrderService)
  id = input<string>();
  
  order = rxResource({
    params: () => {
      const id = this.id();
      return id ? Number(id) : null;
    },
    stream: ({ params }) => {
      if (params === null) {
        return EMPTY;
      }

      return this.orderService.getAdminOrderById(params);
    }
  });
}
