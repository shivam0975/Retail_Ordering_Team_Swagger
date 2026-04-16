namespace backend.Dtos;

public class AdminCategoryDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

public class AdminBrandDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

public class AdminCouponDto
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public decimal Discount { get; set; }

    public DateTime? ExpiryDate { get; set; }
}

public class AdminProductUpsertDto
{
    public string Name { get; set; } = string.Empty;

    public int? CategoryId { get; set; }

    public int? BrandId { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; } = true;

    public int Stock { get; set; }
}

public class AdminCategoryUpsertDto
{
    public string Name { get; set; } = string.Empty;
}

public class AdminBrandUpsertDto
{
    public string Name { get; set; } = string.Empty;
}

public class AdminCouponUpsertDto
{
    public string Code { get; set; } = string.Empty;

    public decimal Discount { get; set; }

    public DateTime? ExpiryDate { get; set; }
}

public class AdminOrderItemDto
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Price { get; set; }
}

public class AdminOrderDto
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string UserEmail { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string DeliveryAddress { get; set; } = string.Empty;

    public List<AdminOrderItemDto> Items { get; set; } = [];
}

public class AdminOrderStatusUpdateDto
{
    public string Status { get; set; } = string.Empty;
}
