using System.Reflection;
using EnforcedVariables.Attributes;

namespace EnforcedVariables.Extensions;

/// <summary>
/// Extensions for <see cref="Assembly"/>.
/// </summary>
internal static class AssemblyExtensions
{
    /// <summary>
    /// Get classes from the provided assembly that are decorated with
    /// <see cref="EnforcedVariablesAttribute"/> attribute.
    /// </summary>
    internal static Type[] GetEnforcedVariablesClasses(this Assembly assembly)
    {
        var types = assembly.GetTypes();

        return types.Where(x => x.IsEnforcedVariablesClass()).ToArray();
    }
}