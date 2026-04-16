import { Routes } from '@angular/router';
import { AdminCatalogPage } from './pages/admin/catalog/admin-catalog-page';
import { AdminDashboardPage } from './pages/admin/dashboard/admin-dashboard-page';
import { AdminLoginPage } from './pages/admin/login/admin-login-page';
import { AdminOrdersPage } from './pages/admin/orders/admin-orders-page';
import { AdminProductsPage } from './pages/admin/products/admin-products-page';
import { CartPage } from './pages/cart/cart-page';
import { DashboardPage } from './pages/dashboard/dashboard-page';
import { LoginPage } from './pages/login/login-page';
import { MenuPage } from './pages/menu/menu-page';
import { OrdersPage } from './pages/orders/orders-page';
import { adminGuard, adminGuestGuard, authGuard, guestGuard } from './state/auth-guard';

export const routes: Routes = [
	{ path: '', pathMatch: 'full', redirectTo: 'login' },
	{ path: 'admin/login', component: AdminLoginPage, canActivate: [adminGuestGuard] },
	{ path: 'admin/dashboard', component: AdminDashboardPage, canActivate: [adminGuard] },
	{ path: 'admin/products', component: AdminProductsPage, canActivate: [adminGuard] },
	{ path: 'admin/catalog', component: AdminCatalogPage, canActivate: [adminGuard] },
	{ path: 'admin/orders', component: AdminOrdersPage, canActivate: [adminGuard] },
	{ path: 'login', component: LoginPage, canActivate: [guestGuard] },
	{ path: 'menu', component: MenuPage, canActivate: [authGuard] },
	{ path: 'cart', component: CartPage, canActivate: [authGuard] },
	{ path: 'orders', component: OrdersPage, canActivate: [authGuard] },
	{ path: 'dashboard', component: DashboardPage, canActivate: [authGuard] },
	{ path: '**', redirectTo: 'login' }
];
