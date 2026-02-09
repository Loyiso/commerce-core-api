namespace CartService.API.Contracts;

public class CartResponse
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public List<CartItemResponse> Items { get; set; } = new();
}

public class CartItemResponse
{
    public Guid Id { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime AddedAtUtc { get; set; }
}
