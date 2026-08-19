namespace DotNetLibraries.FluentValidation;

public static class Extensions
{
    public static IRuleBuilder<TEntity, string> NotEmpty<TEntity>(this IRuleBuilder<TEntity, string> builder)
    {
        Func<TEntity, ValidationError?> function = instance =>
        {
            var value = (string)builder._getter(instance);

            if (string.IsNullOrEmpty(value))
            {
                return new ValidationError(builder._propertyName, "NotEmpty", $"{builder._propertyName} cannot be null or empty");
            }

            return null;
        };

        builder._funcs.Add(function!);
        return builder;
    }

    public static IRuleBuilder<TEntity, decimal> GreaterThan<TEntity>(this IRuleBuilder<TEntity, decimal> builder, decimal max)
    {
        Func<TEntity, ValidationError?> function = instance =>
        {
            var value = (decimal)builder._getter(instance);

            if (value <= max)
            {
                return new ValidationError(builder._propertyName, "GreaterThan", $"{builder._propertyName} must be greater than {max}");
            }

            return null;
        };

        builder._funcs.Add(function!);
        return builder;
    }

    public static IRuleBuilder<TEntity, TProperty> WithMessage<TEntity, TProperty>(this IRuleBuilder<TEntity, TProperty> builder, string message)
    {
        var lastIndex = builder._funcs.Count - 1;
        var lastRule = builder._funcs[lastIndex];

        builder._funcs[lastIndex] = instance =>
        {
            var failure = lastRule(instance);

            if (failure is not null)
            {
                return new(failure.PropertyName, failure.ErrorCode, message);
            }

            return null;
        };

        return builder;
    }
}