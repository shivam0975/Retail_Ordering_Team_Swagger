namespace backend.Dtos;

public class CheckoutRequestDto
{
    public int UserId { get; set; }

    public string DeliveryAddress { get; set; } = string.Empty;

    public string? CouponCode { get; set; }

    public string PaymentMethod { get; set; } = "CASH";
}

public class OrderItemViewDto
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Price { get; set; }
}

public class OrderViewDto
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "PLACED";

    public DateTime CreatedAt { get; set; }

    public string DeliveryAddress { get; set; } = string.Empty;

    public List<OrderItemViewDto> Items { get; set; } = [];
}
