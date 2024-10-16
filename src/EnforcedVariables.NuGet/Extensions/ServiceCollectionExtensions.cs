using System.Reflection;
using EnforcedVariables.Attributes;
using EnforcedVariables.Exceptions;
using EnforcedVariables.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnforcedVariables.Extensions;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/> around enforcing variables.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds any classes decorated with <see cref="EnforcedVariablesAttribute"/> to the
    /// <see cref="IServiceCollection"/>, ensuring all variables (properties) are present and resolvable.
    /// Throws a <see cref="MissingVariablesException"/> if any variables are missing and <see cref="throwOnMissing"/>
    /// is true.
    /// </summary>
    /// <param name="services">The service collection to add variable classes to.</param>
    /// <param name="configuration">The configuration containing the variables to check for.</param>
    /// <param name="throwOnMissing">
    /// If true, throws a <see cref="MissingVariablesException"/> on variables missing.
    /// </param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="MissingVariablesException">
    /// Indicates any missing variables not found in the <see cref="IConfiguration"/>
    /// (if <see cref="throwOnMissing"/> is true).
    /// </exception>
    public static IServiceCollection AddEnforcedVariableClasses(this IServiceCollection services,
        IConfiguration configuration, bool throwOnMissing = false)
    {
        var assembly = Assembly.GetCallingAssembly();
        foreach (var type in assembly.GetEnforcedVariablesClasses())
        {
            EnforcedVariableUtilities.EnforceVariables(services, configuration, type, throwOnMissing, true);
        }

        return services;
    }
    
    /// <summary>
    /// Ensures all variables are present in the <see cref="IConfiguration"/>. Adds the <see cref="TClass"/> to the
    /// <see cref="IServiceCollection"/> as a singleton if all variables are present. If <see cref="throwOnMissing"/>
    /// is true, throws an <see cref="MissingVariablesException"/> if any variables are missing.
    /// </summary>
    /// <param name="services">
    /// The service collection to add the <see cref="TClass"/> to.
    /// </param>
    /// <param name="configuration">The configuration containing the variables to check for.</param>
    /// <param name="throwOnMissing"></param>
    /// <typeparam name="TClass">The class containing variables (properties) to verify.</typeparam>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="MissingVariablesException">
    /// Indicates any missing variables not found in the <see cref="IConfiguration"/> (if <see cref="throwOnMissing"/>
    /// is true).
    /// </exception>
    public static IServiceCollection AddVariables<TClass>(this IServiceCollection services, 
        IConfiguration configuration, bool throwOnMissing = true) where TClass : class, new() =>
            EnforcedVariableUtilities.EnforceVariables(services, configuration, typeof(TClass), throwOnMissing, true);

    /// <summary>
    /// Ensures all variables are present in the <see cref="IConfiguration"/>. Adds the <see cref="classType"/> to the
    /// <see cref="IServiceCollection"/> as a singleton if all variables are present. If <see cref="throwOnMissing"/>
    /// is true, throws an <see cref="MissingVariablesException"/> if any variables are missing.
    /// </summary>
    /// <param name="services">
    /// The service collection to add the <see cref="classType"/> to.
    /// </param>
    /// <param name="configuration">The configuration containing the variables to check for.</param>
    /// <param name="classType">The class containing variables (properties) to verify.</param>
    /// <param name="throwOnMissing">
    /// If true, throws a <see cref="MissingVariablesException"/> on variables missing.
    /// </param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="MissingVariablesException">
    /// Indicates any missing variables not found in the <see cref="IConfiguration"/> (if <see cref="throwOnMissing"/>
    /// is true).
    /// </exception>
    public static IServiceCollection EnforceVariables(this IServiceCollection services, IConfiguration configuration,
        Type classType, bool throwOnMissing = true) => 
        EnforcedVariableUtilities.EnforceVariables(services, configuration, classType, throwOnMissing, true);
}