import { Routes } from "@angular/router";
import { OrdersOverview } from "./orders-overview/orders-overview";
import { Checkout } from "./checkout/checkout";
import { OrderDetails } from "./order-details/order-details";
import { AdminOrders } from "./admin-orders/admin-orders";
import { adminGuard } from "../../core/guards/admin-guard";
import { authGuard } from "../../core/guards/auth-guard";
import { AdminOrderDetails } from "./admin-order-details/admin-order-details";

export const OrdersRoutes: Routes = [
    { path: '', component: OrdersOverview },
    { path: 'checkout', component: Checkout },
    { path: 'admin', component: AdminOrders, canActivate: [authGuard, adminGuard]},
    { path: 'admin/:id', component: AdminOrderDetails, canActivate: [authGuard, adminGuard]},
    { path: ':id', component: OrderDetails }
];