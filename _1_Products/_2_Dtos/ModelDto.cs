using Entities;

namespace Dtos;
public class ModelDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ThumbUrl { get; set; }
    public string PhotoUrl { get; set; }
    public Category Category { get; set; }
    public Guid ProductId { get; set; }
    public Guid BrandId { get; set; }
    public long RatingCount { get; set; } = 0;
    public double RatingAverage { get; set; } = 0.0;
    public StockDto Stock { get; set; }
    public List<PurchaseItemDto> PurchasesItems { get; set; }
    public List<SaleItemDto> SalesItems { get; set; }
}
