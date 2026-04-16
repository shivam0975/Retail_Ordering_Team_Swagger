using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class AdminService(RetailOrderingContext context) : IAdminService
{
    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
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

    public async Task<List<ProductListItemDto>> GetProductsAsync()
    {
        return await context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Inventory)
            .OrderBy(p => p.Name)
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                BrandId = p.BrandId,
                BrandName = p.Brand != null ? p.Brand.Name : string.Empty,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                IsAvailable = p.IsAvailable ?? true,
                Stock = p.Inventory != null ? p.Inventory.Stock ?? 0 : 0
            })
            .ToListAsync();
    }

    public async Task<ServiceResult<ProductListItemDto>> CreateProductAsync(AdminProductUpsertDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Price < 0 || request.Stock < 0)
        {
            return ServiceResult<ProductListItemDto>.Fail("Name, valid price, and valid stock are required.");
        }

        var product = new Product
        {
            Name = request.Name.Trim(),
            CategoryId = request.CategoryId,
            BrandId = request.BrandId,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            IsAvailable = request.IsAvailable
        };

        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        var inventory = new Inventory
        {
            ProductId = product.Id,
            Stock = request.Stock
        };

        await context.Inventories.AddAsync(inventory);
        await context.SaveChangesAsync();

        var created = await MapProductAsync(product.Id);
        return ServiceResult<ProductListItemDto>.Ok(created, "Product created successfully.");
    }

    public async Task<ServiceResult<ProductListItemDto>> UpdateProductAsync(int productId, AdminProductUpsertDto request)
    {
        var product = await context.Products
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product is null)
        {
            return ServiceResult<ProductListItemDto>.Fail("Product not found.");
        }

        if (string.IsNullOrWhiteSpace(request.Name) || request.Price < 0 || request.Stock < 0)
        {
            return ServiceResult<ProductListItemDto>.Fail("Name, valid price, and valid stock are required.");
        }

        product.Name = request.Name.Trim();
        product.CategoryId = request.CategoryId;
        product.BrandId = request.BrandId;
        product.Price = request.Price;
        product.ImageUrl = request.ImageUrl;
        product.IsAvailable = request.IsAvailable;

        if (product.Inventory is null)
        {
            product.Inventory = new Inventory
            {
                ProductId = product.Id,
                Stock = request.Stock
            };
        }
        else
        {
            product.Inventory.Stock = request.Stock;
        }

        await context.SaveChangesAsync();

        var updated = await MapProductAsync(product.Id);
        return ServiceResult<ProductListItemDto>.Ok(updated, "Product updated successfully.");
    }

    public async Task<ServiceResult<bool>> DeleteProductAsync(int productId)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product is null)
        {
            return ServiceResult<bool>.Fail("Product not found.");
        }

        context.Products.Remove(product);
        await context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true, "Product deleted successfully.");
    }

    public async Task<List<AdminCategoryDto>> GetCategoriesAsync()
    {
        return await context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new AdminCategoryDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }

    public async Task<ServiceResult<AdminCategoryDto>> CreateCategoryAsync(AdminCategoryUpsertDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ServiceResult<AdminCategoryDto>.Fail("Category name is required.");
        }

        var name = request.Name.Trim();

        var exists = await context.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower());
        if (exists)
        {
            return ServiceResult<AdminCategoryDto>.Fail("Category already exists.");
        }

        var category = new Category
        {
            Name = name
        };

        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        return ServiceResult<AdminCategoryDto>.Ok(new AdminCategoryDto
        {
            Id = category.Id,
            Name = category.Name
        }, "Category created successfully.");
    }

    public async Task<ServiceResult<AdminCategoryDto>> UpdateCategoryAsync(int categoryId, AdminCategoryUpsertDto request)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
        if (category is null)
        {
            return ServiceResult<AdminCategoryDto>.Fail("Category not found.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ServiceResult<AdminCategoryDto>.Fail("Category name is required.");
        }

        category.Name = request.Name.Trim();
        await context.SaveChangesAsync();

        return ServiceResult<AdminCategoryDto>.Ok(new AdminCategoryDto
        {
            Id = category.Id,
            Name = category.Name
        }, "Category updated successfully.");
    }

    public async Task<ServiceResult<bool>> DeleteCategoryAsync(int categoryId)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
        if (category is null)
        {
            return ServiceResult<bool>.Fail("Category not found.");
        }

        context.Categories.Remove(category);
        await context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true, "Category deleted successfully.");
    }

    public async Task<List<AdminBrandDto>> GetBrandsAsync()
    {
        return await context.Brands
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .Select(b => new AdminBrandDto
            {
                Id = b.Id,
                Name = b.Name
            })
            .ToListAsync();
    }

    public async Task<ServiceResult<AdminBrandDto>> CreateBrandAsync(AdminBrandUpsertDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ServiceResult<AdminBrandDto>.Fail("Brand name is required.");
        }

        var name = request.Name.Trim();

        var exists = await context.Brands.AnyAsync(b => b.Name.ToLower() == name.ToLower());
        if (exists)
        {
            return ServiceResult<AdminBrandDto>.Fail("Brand already exists.");
        }

        var brand = new Brand
        {
            Name = name
        };

        await context.Brands.AddAsync(brand);
        await context.SaveChangesAsync();

        return ServiceResult<AdminBrandDto>.Ok(new AdminBrandDto
        {
            Id = brand.Id,
            Name = brand.Name
        }, "Brand created successfully.");
    }

    public async Task<ServiceResult<AdminBrandDto>> UpdateBrandAsync(int brandId, AdminBrandUpsertDto request)
    {
        var brand = await context.Brands.FirstOrDefaultAsync(b => b.Id == brandId);
        if (brand is null)
        {
            return ServiceResult<AdminBrandDto>.Fail("Brand not found.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ServiceResult<AdminBrandDto>.Fail("Brand name is required.");
        }

        brand.Name = request.Name.Trim();
        await context.SaveChangesAsync();

        return ServiceResult<AdminBrandDto>.Ok(new AdminBrandDto
        {
            Id = brand.Id,
            Name = brand.Name
        }, "Brand updated successfully.");
    }

    public async Task<ServiceResult<bool>> DeleteBrandAsync(int brandId)
    {
        var brand = await context.Brands.FirstOrDefaultAsync(b => b.Id == brandId);
        if (brand is null)
        {
            return ServiceResult<bool>.Fail("Brand not found.");
        }

        context.Brands.Remove(brand);
        await context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true, "Brand deleted successfully.");
    }

    public async Task<List<AdminCouponDto>> GetCouponsAsync()
    {
        return await context.Coupons
            .AsNoTracking()
            .OrderByDescending(c => c.ExpiryDate)
            .Select(c => new AdminCouponDto
            {
                Id = c.Id,
                Code = c.Code ?? string.Empty,
                Discount = c.Discount ?? 0,
                ExpiryDate = c.ExpiryDate
            })
            .ToListAsync();
    }

    public async Task<ServiceResult<AdminCouponDto>> CreateCouponAsync(AdminCouponUpsertDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Code) || request.Discount <= 0)
        {
            return ServiceResult<AdminCouponDto>.Fail("Coupon code and valid discount are required.");
        }

        var code = request.Code.Trim().ToUpperInvariant();

        var exists = await context.Coupons.AnyAsync(c => c.Code == code);
        if (exists)
        {
            return ServiceResult<AdminCouponDto>.Fail("Coupon code already exists.");
        }

        var coupon = new Coupon
        {
            Code = code,
            Discount = request.Discount,
            ExpiryDate = request.ExpiryDate
        };

        await context.Coupons.AddAsync(coupon);
        await context.SaveChangesAsync();

        return ServiceResult<AdminCouponDto>.Ok(new AdminCouponDto
        {
            Id = coupon.Id,
            Code = coupon.Code ?? string.Empty,
            Discount = coupon.Discount ?? 0,
            ExpiryDate = coupon.ExpiryDate
        }, "Coupon created successfully.");
    }

    public async Task<ServiceResult<AdminCouponDto>> UpdateCouponAsync(int couponId, AdminCouponUpsertDto request)
    {
        var coupon = await context.Coupons.FirstOrDefaultAsync(c => c.Id == couponId);
        if (coupon is null)
        {
            return ServiceResult<AdminCouponDto>.Fail("Coupon not found.");
        }

        if (string.IsNullOrWhiteSpace(request.Code) || request.Discount <= 0)
        {
            return ServiceResult<AdminCouponDto>.Fail("Coupon code and valid discount are required.");
        }

        coupon.Code = request.Code.Trim().ToUpperInvariant();
        coupon.Discount = request.Discount;
        coupon.ExpiryDate = request.ExpiryDate;
        await context.SaveChangesAsync();

        return ServiceResult<AdminCouponDto>.Ok(new AdminCouponDto
        {
            Id = coupon.Id,
            Code = coupon.Code ?? string.Empty,
            Discount = coupon.Discount ?? 0,
            ExpiryDate = coupon.ExpiryDate
        }, "Coupon updated successfully.");
    }

    public async Task<ServiceResult<bool>> DeleteCouponAsync(int couponId)
    {
        var coupon = await context.Coupons.FirstOrDefaultAsync(c => c.Id == couponId);
        if (coupon is null)
        {
            return ServiceResult<bool>.Fail("Coupon not found.");
        }

        context.Coupons.Remove(coupon);
        await context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true, "Coupon deleted successfully.");
    }

    public async Task<List<AdminOrderDto>> GetOrdersAsync()
    {
        return await context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new AdminOrderDto
            {
                OrderId = o.Id,
                UserId = o.UserId ?? 0,
                UserName = o.User != null ? o.User.Name : string.Empty,
                UserEmail = o.User != null ? o.User.Email : string.Empty,
                TotalAmount = o.TotalAmount ?? 0,
                Status = o.Status ?? string.Empty,
                CreatedAt = o.CreatedAt ?? DateTime.UtcNow,
                DeliveryAddress = o.DeliveryAddress ?? string.Empty,
                Items = o.OrderItems.Select(oi => new AdminOrderItemDto
                {
                    ProductId = oi.ProductId ?? 0,
                    ProductName = oi.Product != null ? oi.Product.Name : string.Empty,
                    Quantity = oi.Quantity,
                    Price = oi.Price ?? 0
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<ServiceResult<AdminOrderDto>> UpdateOrderStatusAsync(int orderId, AdminOrderStatusUpdateDto request)
    {
        var order = await context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null)
        {
            return ServiceResult<AdminOrderDto>.Fail("Order not found.");
        }

        if (string.IsNullOrWhiteSpace(request.Status))
        {
            return ServiceResult<AdminOrderDto>.Fail("Order status is required.");
        }

        order.Status = request.Status.Trim().ToUpperInvariant();

        await context.OrderStatusHistories.AddAsync(new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = order.Status,
            UpdatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();

        return ServiceResult<AdminOrderDto>.Ok(new AdminOrderDto
        {
            OrderId = order.Id,
            UserId = order.UserId ?? 0,
            UserName = order.User?.Name ?? string.Empty,
            UserEmail = order.User?.Email ?? string.Empty,
            TotalAmount = order.TotalAmount ?? 0,
            Status = order.Status ?? string.Empty,
            CreatedAt = order.CreatedAt ?? DateTime.UtcNow,
            DeliveryAddress = order.DeliveryAddress ?? string.Empty,
            Items = order.OrderItems.Select(oi => new AdminOrderItemDto
            {
                ProductId = oi.ProductId ?? 0,
                ProductName = oi.Product?.Name ?? string.Empty,
                Quantity = oi.Quantity,
                Price = oi.Price ?? 0
            }).ToList()
        }, "Order status updated successfully.");
    }

    private async Task<ProductListItemDto> MapProductAsync(int productId)
    {
        return await context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Inventory)
            .Where(p => p.Id == productId)
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                BrandId = p.BrandId,
                BrandName = p.Brand != null ? p.Brand.Name : string.Empty,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                IsAvailable = p.IsAvailable ?? true,
                Stock = p.Inventory != null ? p.Inventory.Stock ?? 0 : 0
            })
            .FirstAsync();
    }
}
