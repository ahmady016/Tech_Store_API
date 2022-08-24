using Entities;

namespace Dtos;
public class ProductDto
{
  public Guid Id { get; set; }
  public string Title { get; set; }
  public string Description { get; set; }
  public Category Category { get; set; }
}
