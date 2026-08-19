using DotNetLibraries.FluentValidation;
using System.Reflection;

namespace DotNetLibraries.WebAPI.Attributes;

public sealed class ValidateFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var body = context.Arguments.FirstOrDefault();
        if (body is null) return await next(context);

        var bodyType = body.GetType();
        var validatorBaseType = typeof(AbstractValidator<>).MakeGenericType(bodyType);

        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && validatorBaseType.IsAssignableFrom(t));

        List<ValidationResult> validationResults = new();
        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);
            if (instance is null) continue;

            MethodInfo? methodInfo = type.GetMethod("Validate");
            if (methodInfo is null) continue;
            ValidationResult result = (ValidationResult)methodInfo.Invoke(instance, [body])!;
            if (!result.IsValid) validationResults.Add(result);
        }

        if (validationResults.Any())
        {
            return Results.BadRequest(validationResults.SelectMany(r => r.Errors).Distinct());
        }

        return await next(context);
    }
}
