namespace Dtos;
public class PurchaseDto
{
    public Guid Id { get; set; }
    public DateTime PurchasedAt { get; set; }
    public double TotalPrice { get; set; } = 0.0;
    public Guid EmployeeId { get; set; }
    public List<PurchaseItemDto> Items { get; set; }
}
