namespace Dtos;

public class DetailedSaleItemDto : SaleItemDto
{
    public string ModelTitle { get; set; }
    public DateTime SoldAt { get; set; }
    public string EmployeeId { get; set; }
    public string CustomerId { get; set; }
}
