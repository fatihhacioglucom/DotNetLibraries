using DotNetLibraries.FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace DotNetLibraries.WebAPI.Attributes;

public sealed class ValidateAttribute : Attribute, IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context) { }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var body = context.ActionArguments.FirstOrDefault().Value;
        if (body is null) return;

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
            context.Result = new ObjectResult(validationResults.SelectMany(r => r.Errors).Distinct())
            {
                StatusCode = StatusCodes.Status400BadRequest
            };

            return;
        }
    }
}
