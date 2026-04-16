using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class OrderService(RetailOrderingContext context) : IOrderService
{
    public async Task<ServiceResult<OrderViewDto>> CheckoutAsync(CheckoutRequestDto request)
    {
        if (request.UserId <= 0 || string.IsNullOrWhiteSpace(request.DeliveryAddress))
        {
            return ServiceResult<OrderViewDto>.Fail("User and delivery address are required.");
        }

        var cart = await context.Carts
            .FirstOrDefaultAsync(c => c.UserId == request.UserId);

        if (cart is null)
        {
            return ServiceResult<OrderViewDto>.Fail("Cart not found for the user.");
        }

        var cartItems = await context.CartItems
            .Where(ci => ci.CartId == cart.Id)
            .Include(ci => ci.Product)
            .ThenInclude(p => p!.Inventory)
            .ToListAsync();

        if (cartItems.Count == 0)
        {
            return ServiceResult<OrderViewDto>.Fail("Cart is empty.");
        }

        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            foreach (var item in cartItems)
            {
                var stock = item.Product?.Inventory?.Stock ?? 0;
                if (stock < item.Quantity)
                {
                    await transaction.RollbackAsync();
                    return ServiceResult<OrderViewDto>.Fail($"Insufficient stock for product {item.Product?.Name}.");
                }
            }

            var subTotal = cartItems.Sum(ci => (ci.UnitPrice ?? 0) * ci.Quantity);

            int? couponId = null;
            decimal discountAmount = 0;

            if (!string.IsNullOrWhiteSpace(request.CouponCode))
            {
                var coupon = await context.Coupons.FirstOrDefaultAsync(c => c.Code == request.CouponCode);
                if (coupon is not null && coupon.ExpiryDate >= DateTime.UtcNow)
                {
                    couponId = coupon.Id;
                    discountAmount = (coupon.Discount ?? 0) / 100m * subTotal;
                }
            }

            var order = new Order
            {
                UserId = request.UserId,
                TotalAmount = Math.Max(0, subTotal - discountAmount),
                Status = "PLACED",
                CouponId = couponId,
                CreatedAt = DateTime.UtcNow,
                DeliveryAddress = request.DeliveryAddress
            };

            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.UnitPrice ?? 0
                };

                await context.OrderItems.AddAsync(orderItem);

                if (cartItem.Product?.Inventory is not null)
                {
                    cartItem.Product.Inventory.Stock = (cartItem.Product.Inventory.Stock ?? 0) - cartItem.Quantity;
                }
            }

            await context.OrderStatusHistories.AddAsync(new OrderStatusHistory
            {
                OrderId = order.Id,
                Status = "PLACED",
                UpdatedAt = DateTime.UtcNow
            });

            await context.Payments.AddAsync(new Payment
            {
                OrderId = order.Id,
                Amount = order.TotalAmount,
                Method = request.PaymentMethod,
                Status = "PENDING"
            });

            context.CartItems.RemoveRange(cartItems);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            var createdOrder = await BuildOrderViewAsync(order.Id);

            return ServiceResult<OrderViewDto>.Ok(createdOrder, "Order placed successfully.");
        }
        catch
        {
            await transaction.RollbackAsync();
            return ServiceResult<OrderViewDto>.Fail("Failed to place order.");
        }
    }

    public async Task<List<OrderViewDto>> GetOrdersByUserAsync(int userId)
    {
        return await context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderViewDto
            {
                OrderId = o.Id,
                UserId = o.UserId ?? 0,
                TotalAmount = o.TotalAmount ?? 0,
                Status = o.Status ?? "PLACED",
                CreatedAt = o.CreatedAt ?? DateTime.UtcNow,
                DeliveryAddress = o.DeliveryAddress ?? string.Empty,
                Items = o.OrderItems
                    .Select(oi => new OrderItemViewDto
                    {
                        ProductId = oi.ProductId ?? 0,
                        ProductName = oi.Product != null ? oi.Product.Name : string.Empty,
                        Quantity = oi.Quantity,
                        Price = oi.Price ?? 0
                    })
                    .ToList()
            })
            .ToListAsync();
    }

    private async Task<OrderViewDto> BuildOrderViewAsync(int orderId)
    {
        var order = await context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstAsync(o => o.Id == orderId);

        return new OrderViewDto
        {
            OrderId = order.Id,
            UserId = order.UserId ?? 0,
            TotalAmount = order.TotalAmount ?? 0,
            Status = order.Status ?? "PLACED",
            CreatedAt = order.CreatedAt ?? DateTime.UtcNow,
            DeliveryAddress = order.DeliveryAddress ?? string.Empty,
            Items = order.OrderItems.Select(oi => new OrderItemViewDto
            {
                ProductId = oi.ProductId ?? 0,
                ProductName = oi.Product != null ? oi.Product.Name : string.Empty,
                Quantity = oi.Quantity,
                Price = oi.Price ?? 0
            }).ToList()
        };
    }
}
