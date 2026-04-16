namespace backend.Dtos;

public class AddCartItemRequestDto
{
    public int UserId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }
}

public class UpdateCartItemRequestDto
{
    public int Quantity { get; set; }
}

public class CartItemViewDto
{
    public int CartItemId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal { get; set; }
}

public class CartViewDto
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public List<CartItemViewDto> Items { get; set; } = [];

    public decimal SubTotal { get; set; }
}
