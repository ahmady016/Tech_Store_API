namespace Dtos;

public class StockDto
{
    public Guid ModelId { get; set; }
    public double TotalPurchasesPrice { get; set; } = 0.0;
    public double TotalSalesPrice { get; set; } = 0.0;
    public double Profit { get; set; } = 0.0;
    public long TotalPurchasesQuantity { get; set; } = 0;
    public long TotalSalesQuantity { get; set; } = 0;
    public long TotalInStock { get; set; } = 0;

    public ModelDto Model { get; set; }
}
