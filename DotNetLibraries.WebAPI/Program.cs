using DotNetLibraries.WebAPI.Attributes;
using DotNetLibraries.WebAPI.Dtos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/products", (CreateProductDto request) =>
{
    return Results.Ok(new { Message = $"Product created successfully!" });
})
    .AddEndpointFilter<ValidateFilter>();

app.MapControllers();

app.Run();