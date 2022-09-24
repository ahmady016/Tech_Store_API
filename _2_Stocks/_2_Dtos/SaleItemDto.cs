namespace Dtos;
public class SaleItemDto
{
    public Guid Id { get; set; }
    public int Quantity { get; set; } = 0;
    public double UnitPrice { get; set; } = 0.0;
    public double TotalPrice { get; set; } = 0.0;
    public Guid ModelId { get; set; }
    public Guid SaleId { get; set; }
}
