using System.Collections.Concurrent;
using System.Reflection;

namespace DotNetLibraries.Mapster;

public static class Extensions
{
    private static readonly ConcurrentDictionary<(Type source, Type destination), Action<object, object>> _mapCache = new();

    private static Action<object, object> GetMapAction(Type source, Type destination) => _mapCache.GetOrAdd((source, destination), Build);

    private static Action<object, object> Build((Type source, Type destination) key)
    {
        var sourceProperties = key.source
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProperties = key.destination.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToDictionary(p => p.Name);

        var pairs = new List<(PropertyInfo source, PropertyInfo destination)>();
        foreach (var sourceProperty in sourceProperties)
        {
            if (!destinationProperties.TryGetValue(sourceProperty.Name, out var destinationProperty)) continue;
            if (!destinationProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType)) continue;

            pairs.Add((sourceProperty, destinationProperty));
        }

        return (sourceObject, destinationObject) =>
        {
            foreach (var (sourceProperty, destinationProperty) in pairs)
            {
                var value = sourceProperty.GetValue(sourceObject);
                destinationProperty.SetValue(destinationObject, value);
            }
        };
    }

    private static TEntity MapTo<TEntity>(object source, TEntity instance) where TEntity : class, new()
    {
        var sourceProperties = source.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProperties = instance.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToList();

        foreach (var sourceProperty in sourceProperties)
        {
            var destinationProperty = destinationProperties
                .FirstOrDefault(p => p.Name == sourceProperty.Name);
            if (destinationProperty is null) continue;
            if (destinationProperty.PropertyType != sourceProperty.PropertyType) continue;

            destinationProperty.SetValue(instance, sourceProperty.GetValue(source));
        }

        return instance;
    }

    public static TEntity Adapt<TEntity>(this object source) where TEntity : class, new()
    {
        var instance = new TEntity();
        GetMapAction(source.GetType(), instance.GetType())(source, instance);
        return MapTo(source, instance);
    }

    public static TEntity Adapt<TEntity>(this object source, TEntity destination) where TEntity : class, new()
    {
        GetMapAction(source.GetType(), destination.GetType())(source, destination);
        return MapTo(source, destination);
    }
}