import { Component, inject, input } from '@angular/core';
import { OrderService } from '../order-service';
import { rxResource } from '@angular/core/rxjs-interop';
import { DatePipe } from '@angular/common';
import { EMPTY } from 'rxjs';

@Component({
  selector: 'app-order-details',
  imports: [DatePipe],
  templateUrl: './order-details.html',
  styleUrl: './order-details.css',
})
export class OrderDetails {
  private orderService = inject(OrderService);

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
      return this.orderService.getOrderById(params);
    }
  });


  cancelOrder(id:number) {
    if (!confirm("Cancel this order?")) {
      return;
    }

    this.orderService.cancelOrder(id)
      .subscribe(() => this.order.reload());
  }
}
