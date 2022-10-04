using Entities;

namespace Dtos;

public class DetailedStockDto
{
    public Guid ModelId { get; set; }
    public string ModelTitle { get; set; }
    public string ModelDescription { get; set; }
    public string ModelThumbUrl { get; set; }
    public Guid ProductId { get; set; }
    public string ProductTitle { get; set; }
    public Category ProductCategory { get; set; }
    public Guid BrandId { get; set; }
    public string BrandTitle { get; set; }
    public double TotalPurchasesPrice { get; set; } = 0.0;
    public double TotalSalesPrice { get; set; } = 0.0;
    public double Profit { get; set; } = 0.0;
    public long TotalPurchasesQuantity { get; set; } = 0;
    public long TotalSalesQuantity { get; set; } = 0;
    public long TotalInStock { get; set; } = 0;
}
