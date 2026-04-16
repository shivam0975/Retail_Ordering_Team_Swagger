using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class CartService(RetailOrderingContext context) : ICartService
{
    public async Task<ServiceResult<CartViewDto>> GetCartByUserAsync(int userId)
    {
        if (userId <= 0)
        {
            return ServiceResult<CartViewDto>.Fail("Invalid user id.");
        }

        var cart = await GetOrCreateCartAsync(userId);
        var cartView = await BuildCartViewAsync(cart.Id, userId);

        return ServiceResult<CartViewDto>.Ok(cartView, "Cart loaded.");
    }

    public async Task<ServiceResult<CartViewDto>> AddItemAsync(AddCartItemRequestDto request)
    {
        if (request.UserId <= 0 || request.ProductId <= 0 || request.Quantity <= 0)
        {
            return ServiceResult<CartViewDto>.Fail("User, product, and quantity are required.");
        }

        var product = await context.Products
            .AsNoTracking()
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId);

        if (product is null)
        {
            return ServiceResult<CartViewDto>.Fail("Product not found.");
        }

        if (product.IsAvailable == false)
        {
            return ServiceResult<CartViewDto>.Fail("Product is not available.");
        }

        var stock = product.Inventory?.Stock ?? 0;
        if (stock < request.Quantity)
        {
            return ServiceResult<CartViewDto>.Fail("Insufficient stock for requested quantity.");
        }

        var cart = await GetOrCreateCartAsync(request.UserId);

        var cartItem = await context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == request.ProductId);

        if (cartItem is null)
        {
            cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                UnitPrice = product.Price
            };

            await context.CartItems.AddAsync(cartItem);
        }
        else
        {
            var newQuantity = cartItem.Quantity + request.Quantity;
            if (stock < newQuantity)
            {
                return ServiceResult<CartViewDto>.Fail("Insufficient stock for requested quantity.");
            }

            cartItem.Quantity = newQuantity;
            cartItem.UnitPrice = product.Price;
        }

        await context.SaveChangesAsync();

        var updatedCart = await BuildCartViewAsync(cart.Id, request.UserId);
        return ServiceResult<CartViewDto>.Ok(updatedCart, "Item added to cart.");
    }

    public async Task<ServiceResult<CartViewDto>> UpdateItemAsync(int cartItemId, UpdateCartItemRequestDto request)
    {
        if (cartItemId <= 0 || request.Quantity <= 0)
        {
            return ServiceResult<CartViewDto>.Fail("Invalid cart item or quantity.");
        }

        var cartItem = await context.CartItems
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

        if (cartItem is null)
        {
            return ServiceResult<CartViewDto>.Fail("Cart item not found.");
        }

        var product = await context.Products
            .AsNoTracking()
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);

        if (product is null)
        {
            return ServiceResult<CartViewDto>.Fail("Product not found.");
        }

        var stock = product.Inventory?.Stock ?? 0;
        if (stock < request.Quantity)
        {
            return ServiceResult<CartViewDto>.Fail("Insufficient stock for requested quantity.");
        }

        cartItem.Quantity = request.Quantity;
        cartItem.UnitPrice = product.Price;

        await context.SaveChangesAsync();

        var userId = await context.Carts
            .Where(c => c.Id == cartItem.CartId)
            .Select(c => c.UserId)
            .FirstOrDefaultAsync();

        if (!userId.HasValue)
        {
            return ServiceResult<CartViewDto>.Fail("Cart owner not found.");
        }

        var updatedCart = await BuildCartViewAsync(cartItem.CartId ?? 0, userId.Value);
        return ServiceResult<CartViewDto>.Ok(updatedCart, "Cart item updated.");
    }

    public async Task<ServiceResult<bool>> RemoveItemAsync(int cartItemId)
    {
        if (cartItemId <= 0)
        {
            return ServiceResult<bool>.Fail("Invalid cart item id.");
        }

        var cartItem = await context.CartItems.FirstOrDefaultAsync(ci => ci.Id == cartItemId);
        if (cartItem is null)
        {
            return ServiceResult<bool>.Fail("Cart item not found.");
        }

        context.CartItems.Remove(cartItem);
        await context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true, "Cart item removed.");
    }

    private async Task<Cart> GetOrCreateCartAsync(int userId)
    {
        var cart = await context.Carts
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart is not null)
        {
            return cart;
        }

        cart = new Cart
        {
            UserId = userId
        };

        await context.Carts.AddAsync(cart);
        await context.SaveChangesAsync();

        return cart;
    }

    private async Task<CartViewDto> BuildCartViewAsync(int cartId, int userId)
    {
        var items = await context.CartItems
            .AsNoTracking()
            .Where(ci => ci.CartId == cartId)
            .Include(ci => ci.Product)
            .Select(ci => new CartItemViewDto
            {
                CartItemId = ci.Id,
                ProductId = ci.ProductId ?? 0,
                ProductName = ci.Product != null ? ci.Product.Name : string.Empty,
                ImageUrl = ci.Product != null ? ci.Product.ImageUrl : null,
                UnitPrice = ci.UnitPrice ?? 0,
                Quantity = ci.Quantity,
                LineTotal = (ci.UnitPrice ?? 0) * ci.Quantity
            })
            .ToListAsync();

        return new CartViewDto
        {
            CartId = cartId,
            UserId = userId,
            Items = items,
            SubTotal = items.Sum(i => i.LineTotal)
        };
    }
}
