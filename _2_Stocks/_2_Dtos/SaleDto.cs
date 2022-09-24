namespace Dtos;
public class SaleDto
{
    public Guid Id { get; set; }
    public DateTime SoldAt { get; set; }
    public double TotalPrice { get; set; } = 0.0;
    public string EmployeeId { get; set; }
    public string CustomerId { get; set; }
    public List<SaleItemDto> Items { get; set; }
}
