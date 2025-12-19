using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AdoTut;

public class PropertyBuilder<T> where T : class, new()
{
    private T _instance;
    private readonly Dictionary<string, PropertyInfo> _properties;

    public PropertyBuilder()
    {
        _instance = new T();
        _properties = typeof(T).GetProperties()
            .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);
    }



    // Build properties using property expressions for compile-time type checking
    public PropertyBuilder<T> WithProperty<TValue>(
        Expression<Func<T, TValue>> propertyExpression,
        TValue value)
    {
        var memberExpression = (MemberExpression)propertyExpression.Body;
        var property = (PropertyInfo)memberExpression.Member;
        property.SetValue(_instance, value);
        return this;
    }


    // Build properties String-based (RUNTIME TYPE CHECKING)
    public PropertyBuilder<T> WithProperty<TValue>(string propertyName, TValue value)
    {
        if (!_properties.TryGetValue(propertyName, out var property))
        {
            var validProps = string.Join(", ", _properties.Keys);
            throw new ArgumentException(
                $"Property '{propertyName}' does not exist on type {typeof(T).Name}. " +
                $"Valid properties: {validProps}");
        }

        var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
        bool typesMatch = property.PropertyType == typeof(TValue) ||
                          (underlyingType != null && underlyingType == typeof(TValue));

        if (!typesMatch)
        {
            throw new ArgumentException(
                $"Property '{propertyName}' is of type {property.PropertyType.Name}, " +
                $"but value is of type {typeof(TValue).Name}");
        }

        property.SetValue(_instance, value);
        return this;
    }



    public T Build()
    {
        T instance = _instance;
        _instance = new T();
        return instance;
    }


}
