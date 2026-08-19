using DotNetLibraries.FluentValidation;
using DotNetLibraries.WebAPI.Attributes;
using DotNetLibraries.WebAPI.Dtos;
using DotNetLibraries.WebAPI.Validators;
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
        return Ok(new { Message = "Product created successfully." });
    }
}