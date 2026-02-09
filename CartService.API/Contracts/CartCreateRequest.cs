namespace CartService.API.Contracts;

public class CartCreateRequest
{
    public string UserId { get; set; } = string.Empty;

    public List<CartItemUpsertRequest> Items { get; set; } = new();
}

public class CartItemUpsertRequest
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; } = 0m;
}
