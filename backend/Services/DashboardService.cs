using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class DashboardService(RetailOrderingContext context) : IDashboardService
{
    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var totalProducts = await context.Products.CountAsync();
        var totalOrders = await context.Orders.CountAsync();
        var activeUsers = await context.Users.CountAsync();
        var revenue = await context.Orders.SumAsync(o => o.TotalAmount ?? 0);
        var pendingOrders = await context.Orders.CountAsync(o => o.Status == "PLACED" || o.Status == "CONFIRMED");

        return new DashboardSummaryDto
        {
            TotalProducts = totalProducts,
            TotalOrders = totalOrders,
            ActiveUsers = activeUsers,
            Revenue = revenue,
            PendingOrders = pendingOrders
        };
    }
}
