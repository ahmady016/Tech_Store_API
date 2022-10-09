using Entities;

namespace Dtos;

public class CustomerFavoriteModelDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ThumbUrl { get; set; }
    public Category Category { get; set; }
    public Guid ProductId { get; set; }
    public string ProductTitle { get; set; }
    public Guid BrandId { get; set; }
    public string BrandTitle { get; set; }
}
