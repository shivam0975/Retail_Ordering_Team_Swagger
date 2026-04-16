# 🛒 Retail Ordering Website — Pizza, Drinks & Breads

> **Full-Stack .NET — Hackathon MVP**  
> A complete retail ordering platform enabling customers to browse, order, and track Pizza, Cold Drinks, and Breads seamlessly.

---

## 📌 Project Overview

| Attribute | Detail |
|-----------|--------|
| **Project Name** | Retail Ordering Website — Pizza, Drinks & Breads |
| **Classification** | Internal — Use Case 1 |
| **Tech Stack** | ASP.NET Core · Entity Framework Core · MySQL · Angular |
| **Backend URL** | `http://localhost:5142` / `https://localhost:7176` |
| **API Docs** | Swagger UI at `/swagger` (Development mode) |

---

## 🎯 Goal Statement

Enable customers to browse a product menu, add items to a cart, apply coupons, and place orders — with automatic inventory updates, payment tracking, order status history, and secure API operations.

---

## 🏗️ Tech Stack

### Backend
- **Framework:** ASP.NET Core (.NET 10)
- **ORM:** Entity Framework Core with MySQL provider
- **Auth:** JWT-based authentication (admin & customer roles)
- **API Docs:** Swagger / OpenAPI

### Frontend
- **Framework:** Angular (Node.js 24)
- **Build:** Production build via `npm run build -- --configuration production`

### Database
- **Engine:** MySQL
- **Tables:** 12 entities (see Data Model section)

### CI/CD
- **Platform:** GitHub Actions
- **Triggers:** Push & PR to `main`
- **Jobs:** Angular build + .NET build run in parallel

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 24+](https://nodejs.org/)
- MySQL server running locally

### 1. Clone the repository

```bash
git clone https://github.com/shivam0975/Retail_Ordering_Team_Swagger.git
cd Retail_Ordering_Team_Swagger
```

### 2. Configure the database

Update the connection string in `backend/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=retail_ordering;User=root;Password=yourpassword;"
  }
}
```

### 3. Run the backend

```bash
cd backend
dotnet restore
dotnet build
dotnet run
```

API will be available at:
- HTTP: `http://localhost:5142`
- HTTPS: `https://localhost:7176`
- Swagger: `http://localhost:5142/swagger`

### 4. Run the frontend

```bash
cd frontend
npm ci
npm start
```

---

## 📡 API Endpoints

### Auth — `/api/auth`

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/auth/register` | Register a new customer |
| `POST` | `/api/auth/login` | Customer login → returns JWT |
| `POST` | `/api/auth/register-admin` | Register a new admin |
| `POST` | `/api/auth/admin-login` | Admin login → returns JWT |

### Products — `/api/products`

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/products` | List all products |
| `GET` | `/api/products?search=&categoryId=` | Filter by search term or category |

### Cart — `/api/cart`

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/cart/{userId}` | Get cart for a user |
| `POST` | `/api/cart/items` | Add item to cart |
| `PUT` | `/api/cart/items/{cartItemId}` | Update item quantity |
| `DELETE` | `/api/cart/items/{cartItemId}` | Remove item from cart |

### Orders — `/api/orders`

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/orders/checkout` | Place order (decrements stock, records payment) |
| `GET` | `/api/orders/user/{userId}` | Get order history for a user |

### Admin — `/api/admin` *(JWT required)*

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/admin/dashboard` | Dashboard summary |
| `GET` | `/api/admin/products` | List all products |
| `POST` | `/api/admin/products` | Create a product |
| `PUT` | `/api/admin/products/{id}` | Update a product |
| `DELETE` | `/api/admin/products/{id}` | Delete a product |
| `GET` | `/api/admin/orders` | List all orders |
| `PUT` | `/api/admin/orders/{id}/status` | Advance order status |
| `GET` | `/api/admin/inventory` | View inventory levels |
| `PUT` | `/api/admin/inventory/{productId}` | Restock a product |

---

## 📋 User Stories

### Story 1 — Product Catalog Browsing `P0 · MUST HAVE`

Customers can browse products organized by category (Pizza, Drinks, Breads) and brand. Each product card shows name, price, image, and availability. Category and brand filters work independently with page load under 3 seconds.

**Frontend:** `MenuPage` · `ProductCard` · `CategoryFilter` · `BrandFilter`

---

### Story 2 — Inventory Management `P0 · MUST HAVE`

Each product has a tracked stock count that auto-decrements on order placement. Products with zero stock are automatically marked unavailable on the menu.

**Frontend:** `AdminDashboard` · Inventory Table · Low Stock Alert Banner

---

### Story 3 — Shopping Cart `P0 · MUST HAVE`

Users can add/remove items with quantity controls. Cart supports both logged-in users (`user_id`) and guest sessions (`session_id`). The `unit_price` is locked at add-time to prevent price drift. Cart shows live count, subtotal, and total.

**Frontend:** `CartSidebar` / `CartPage` · `CartItem` · Navbar badge

---

### Story 4 — Coupons & Discounts `P1 · SHOULD HAVE`

Users can enter a coupon code at checkout. Valid coupons apply a percentage discount to the total. Expired or invalid codes display an appropriate error message.

**Frontend:** `CouponInput` on `CheckoutPage` · Discount line item in order summary

---

### Story 5 — Order Placement & Payment `P0 · MUST HAVE`

Users submit their cart as a confirmed order with an optional coupon applied. The system returns an Order ID and timestamp, auto-decrements stock, creates a payment record, and logs the initial `PLACED` status in order history. Users land on a success screen with a full order summary.

**Frontend:** `CheckoutPage` · `OrderConfirmation`

---

### Story 6 — Order Tracking `P1 · SHOULD HAVE`

Users see a live status stepper progressing through:

```
PLACED → CONFIRMED → PREPARING → OUT_FOR_DELIVERY → DELIVERED
```

Each status change is timestamped. Admins can advance the order status from the management table.

**Frontend:** `OrderTrackingPage` with stepper · `AdminOrderManagement` table

---

### Story 7 — User Authentication `P1 · SHOULD HAVE`

Customers can register and log in. Login returns a JWT token for session management. Protected routes require a valid token. Admin role gates access to product, inventory, and order management.

**Frontend:** `LoginPage` · `RegisterPage` · `AuthGuard` / `ProtectedRoute`

---

### Story 8 — Order History `P2 · NICE TO HAVE`

Logged-in users can view all past orders, each showing items, total, payment method, and current status. (Reuses data from Story 5 — no new models needed.)

**Frontend:** `OrderHistoryPage` · `OrderCard`

---

### Story 9 — Email Order Confirmation `P2 · NICE TO HAVE` *(Stretch)*

After a successful order, the user receives a confirmation email with their order summary. Triggered via SMTP/SendGrid inside `POST /api/orders/checkout`. No frontend changes needed.

---

## 🗄️ Data Model

### Core Entities

| Table | Key Fields |
|-------|------------|
| **users** | `id`, `name`, `email`, `password_hash`, `role` (CUSTOMER/ADMIN), `created_at` |
| **categories** | `id`, `name` |
| **brands** | `id`, `name` |
| **products** | `id`, `name`, `category_id` (FK), `brand_id` (FK), `price`, `image_url`, `is_available` |
| **inventory** | `id`, `product_id` (UNIQUE FK), `stock` |
| **carts** | `id`, `user_id` (FK, nullable), `session_id` |
| **cart_items** | `id`, `cart_id` (FK), `product_id` (FK), `quantity`, `unit_price` |
| **coupons** | `id`, `code` (UNIQUE), `discount`, `expiry_date` |
| **orders** | `id`, `user_id` (FK), `total_amount`, `status` (ENUM), `coupon_id` (FK), `created_at`, `delivery_address` |
| **order_items** | `id`, `order_id` (FK), `product_id` (FK), `quantity`, `price` |
| **payments** | `id`, `order_id` (FK), `amount`, `status`, `method` |
| **order_status_history** | `id`, `order_id` (FK), `status`, `updated_at` |

### Relationships

| Relationship | Type |
|---|---|
| `users → carts` | 1:N — user can have multiple cart sessions; `user_id` nullable for guests |
| `users → orders` | 1:N — user can place many orders; SET NULL on user delete |
| `categories → products` | 1:N — one category groups many products |
| `brands → products` | 1:N — one brand owns many products |
| `products → inventory` | 1:1 — each product has exactly one inventory row (UNIQUE FK) |
| `carts → cart_items` | 1:N — cart holds many line items; CASCADE on delete |
| `products → cart_items` | 1:N — product can appear in many carts |
| `coupons → orders` | 1:N — one coupon can be applied to many orders |
| `orders → order_items` | 1:N — order contains many line items; CASCADE on delete |
| `orders → payments` | 1:1 (or 1:N for retries) — CASCADE on delete |
| `orders → order_status_history` | 1:N — each status change creates a new history row; CASCADE on delete |

---

## 🏗️ Recommended Build Order

For hackathon scenarios or fresh setup, follow this sequence to maximize demo readiness:

| Step | Task | Why |
|------|------|-----|
| 1 | DB + Models (all 12 tables) | Foundation — all stories depend on schema |
| 2 | Categories, Brands & Product API | Unblocks menu UI and inventory management immediately |
| 3 | Inventory API | Stock tracking required before cart/order logic |
| 4 | Menu UI | Visible progress; ready for demo early |
| 5 | Cart (Frontend + Backend) | Session cart with DB persistence |
| 6 | Coupons API | Discount logic needed before order placement |
| 7 | Order API + Payment | Core business logic — P0 must-have |
| 8 | Order Status History | Enables live tracking screen in demo |
| 9 | Confirmation Screen | Completes the user journey end-to-end |
| 10 | Auth (JWT) | Add JWT layer only after core flow works |

---

## 🎬 Demo Walkthrough

A successful demo shows the following end-to-end journey:

1. **Open the menu page** — products listed by category and brand with name, price, and availability
2. **Add to Cart** — cart badge updates, `unit_price` is locked, subtotal is visible
3. **Enter a coupon code** — discount is applied and reflected in the order total
4. **Submit order** — system confirms with Order ID, timestamp, and payment record created
5. **Open Order Tracking** — show status stepper progressing from `PLACED` to `CONFIRMED`
6. **Show Swagger** — demonstrate stock decrement in inventory, `order_status_history` insert, and secure API response

---

## 📁 Project Structure

```
Retail_Ordering_Team_Swagger/
├── backend/
│   ├── Controllers/
│   │   ├── AdminController.cs
│   │   ├── AuthController.cs
│   │   ├── CartController.cs
│   │   ├── DashboardController.cs
│   │   ├── OrdersController.cs
│   │   └── ProductsController.cs
│   ├── Dtos/
│   ├── Models/
│   │   ├── RetailOrderingContext.cs
│   │   ├── User.cs · Product.cs · Order.cs · Cart.cs
│   │   └── ... (all 12 entity models)
│   ├── Services/
│   │   ├── AuthService.cs · AdminService.cs
│   │   ├── CartService.cs · OrderService.cs
│   │   └── ... (interfaces + implementations)
│   ├── Properties/launchSettings.json
│   └── Program.cs
├── frontend/
│   └── (Angular app)
├── .github/workflows/test.yml
└── backend.sln
```

---

## ⚙️ CI/CD Pipeline

GitHub Actions runs two parallel jobs on every push and PR to `main`:

**Angular Build**
- Sets up Node.js 24
- Runs `npm ci` and `npm run build -- --configuration production`

**.NET Build**
- Sets up .NET 10
- Runs `dotnet restore` and `dotnet build --configuration Release`

---

## 📄 License

Internal use only. Classification: Internal.
