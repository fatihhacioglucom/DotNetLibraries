namespace DotNetLibraries.FluentValidation;

public sealed record ValidationError(
    string PropertyName,
    string ErrorCode,
    string ErrorMessage
);