using EnforcedVariables.Attributes;

namespace EnforcedVariables.Extensions;

/// <summary>
/// Extensions for attributes on a <see cref="Type"/>.
/// </summary>
internal static class AttributeExtensions
{
    /// <summary>
    /// Returns if the provided type has a <see cref="EnforcedVariablesAttribute"/> attribute.
    /// </summary>
    internal static bool IsEnforcedVariablesClass(this Type type)
        => type.IsClassOrStruct() &&
           type.GetCustomAttributes(typeof(EnforcedVariablesAttribute), true).Length != 0;

    /// <summary>
    /// Returns if the provided type is a `class` or `struct`.
    /// </summary>
    internal static bool IsClassOrStruct(this Type type)
        => type.IsClass || type is { IsValueType: true, IsPrimitive: false };
}