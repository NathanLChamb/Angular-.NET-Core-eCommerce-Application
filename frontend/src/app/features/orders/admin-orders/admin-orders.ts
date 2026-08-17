import { Component, computed, inject, signal } from '@angular/core';
import { OrderService } from '../order-service';
import { rxResource } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { OrderSearchFilter, OrderStatus, OrderStatusFilter } from '../models';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-admin-orders',
  imports: [RouterLink, DatePipe],
  templateUrl: './admin-orders.html',
  styleUrl: './admin-orders.css',
})
export class AdminOrders {
  private orderService = inject(OrderService);
  protected pageNumber = signal(1)
  protected pageSize = signal(5)
  protected readonly OrderStatusFilter = OrderStatusFilter;
  protected filter = signal<OrderSearchFilter>({
    status: OrderStatusFilter.All,
    pageNumber: 1,
    pageSize: 10
  });

  protected orders = rxResource({
    params: () => this.filter(),
    stream: ({ params }) => this.orderService.getAllOrders(params)
  });

  protected setStatus(status: OrderStatusFilter) {
    this.filter.update(f => ({
      ...f,
      status,
      pageNumber: 1
    }));
  }

  statuses = [
    { value: OrderStatus.Pending, label: 'Pending' },
    { value: OrderStatus.Processing, label: 'Processing' },
    { value: OrderStatus.Shipped, label: 'Shipped' },
    { value: OrderStatus.Delivered, label: 'Delivered' },
    { value: OrderStatus.Cancelled, label: 'Cancelled' }
  ];

  updateStatus(id: number, status: number) {
    this.orderService.updateOrderStatus(id, status).subscribe(() => {
      this.orders.reload();
    });
  }

  protected totalPages = computed(() => {
    const data = this.orders.value()
    if (!data) return 0

    return Math.ceil(data.totalCount / this.pageSize())
  })

  protected previousPage() {
    this.filter.update(f => ({
      ...f,
      pageNumber: Math.max(1, f.pageNumber - 1)
    }));
  }

  protected nextPage() {
    this.filter.update(f => ({
      ...f,
      pageNumber: f.pageNumber + 1
    }));
  }
}
