namespace Ecommerce.Application.Dto;

public class ProductDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string? ImagePreview { get; set; }
    public string CategoryName { get; set; }
}
