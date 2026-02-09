namespace CartService.API.Contracts;

public class CartUpdateRequest
{
    public string UserId { get; set; } = string.Empty;
 
    public List<CartItemUpsertRequest> Items { get; set; } = new();
}
