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
}
