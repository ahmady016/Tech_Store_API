namespace Dtos;
public class DetailedPurchaseItemDto : PurchaseItemDto
{
    public string ModelTitle { get; set; }
    public DateTime PurchasedAt { get; set; }
    public string EmployeeId { get; set; }
}
