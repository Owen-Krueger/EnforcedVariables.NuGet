using EnforcedVariables.Exceptions;
using EnforcedVariables.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnforcedVariables.Extensions;

/// <summary>
/// Extensions for <see cref="IConfiguration"/> around enforcing variables.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Ensures all variables are present in the <see cref="IConfiguration"/>. Throws an
    /// <see cref="MissingVariablesException"/> if any variables are missing.
    /// </summary>
    /// <param name="configuration">The configuration containing the variables to check for.</param>
    /// <typeparam name="TClass">The class containing variables (properties) to verify.</typeparam>
    /// <exception cref="MissingVariablesException">
    /// Indicates any missing variables not found in the <see cref="IConfiguration"/>.
    /// </exception>
    public static void EnforceVariables<TClass>(this IConfiguration configuration)
        => EnforcedVariableUtilities.EnforceVariables(new ServiceCollection(), configuration, typeof(TClass), true, false);

    /// <summary>
    /// Ensures all variables are present in the <see cref="IConfiguration"/>. Throws an
    /// <see cref="MissingVariablesException"/> if any variables are missing.
    /// </summary>
    /// <param name="configuration">The configuration containing the variables to check for.</param>
    /// <param name="classType">The class containing variables (properties) to verify.</param>
    public static void EnforceVariables(this IConfiguration configuration, Type classType)
        => EnforcedVariableUtilities.EnforceVariables(new ServiceCollection(), configuration, classType, true, false);
}