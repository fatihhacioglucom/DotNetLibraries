using DotNetLibraries.Mapster;
using DotNetLibraries.WebAPI.Attributes;
using DotNetLibraries.WebAPI.Dtos;
using DotNetLibraries.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetLibraries.WebAPI.Controllers;

[ApiController]
[Route("/api/products")]
[Validate]
public sealed class ProductsController : ControllerBase
{
    [HttpPost]
    public IActionResult Create(CreateProductDto request)
    {
        var product = request.Adapt<Product>();
        return Ok(new { Message = "Product created successfully." });
    }

    [HttpPut]
    public IActionResult Update(UpdateProductDto request)
    {
        var product = new Product();
        request.Adapt(product);
        return Ok(new { Message = "Product updated successfully." });
    }
}