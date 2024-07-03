﻿namespace Ecommerce.Application.Dto;

public class FeatureProductShowcaseDto
{
    public long ProductId { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? ShortDescription { get; set; }
    public string? Price { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? ProductImage { get; set; }
    public int Order { get; set; }
}
