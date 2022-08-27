using System.Text.Json.Serialization;
using Entities;
namespace Dtos;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Category Category { get; set; }
    public DateTime CreatedAt { get; set; }
}
