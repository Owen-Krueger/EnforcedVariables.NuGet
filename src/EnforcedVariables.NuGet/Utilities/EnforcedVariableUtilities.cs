using System.Reflection;
using EnforcedVariables.Attributes;
using EnforcedVariables.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnforcedVariables.Utilities;

/// <summary>
/// Utilities for enforcing variables.
/// </summary>
internal static class EnforcedVariableUtilities
{
    /// <summary>
    /// Ensures all variables are present in the <see cref="IConfiguration"/>. If <see cref="throwOnMissing"/> is true,
    /// throws an <see cref="MissingVariablesException"/> if any variables are missing. If <see cref="addSingleton"/>
    /// is true, adds the provided <see cref="classType"/> as a singleton to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">
    /// The service collection to add the <see cref="classType"/> to, if <see cref="addSingleton"/> is true.
    /// </param>
    /// <param name="configuration">The configuration containing the variables to check for.</param>
    /// <param name="classType">The class containing variables (properties) to verify.</param>
    /// <param name="throwOnMissing">
    /// If true, throws a <see cref="MissingVariablesException"/> on variables missing.
    /// </param>
    /// <param name="addSingleton">
    /// If true, adds the <see cref="classType"/> to the <see cref="IServiceCollection"/>.
    /// </param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="MissingVariablesException">
    /// Indicates any missing variables not found in the <see cref="IConfiguration"/> (if <see cref="throwOnMissing"/>
    /// is true).
    /// </exception>
    internal static IServiceCollection EnforceVariables(IServiceCollection services, IConfiguration configuration,
        Type classType, bool throwOnMissing, bool addSingleton)
    {
        var variables = Activator.CreateInstance(classType);
        switch (variables)
        {
            case null when throwOnMissing:
                throw new MissingVariablesException($"Failed to create {classType} type.");
            case null:
                return services;
        }

        var missingProperties = GetMissingVariables(configuration, variables);
        
        if (throwOnMissing && missingProperties.Count > 0)
        {
            throw new MissingVariablesException(missingProperties);
        }
        
        if (addSingleton && missingProperties.Count == 0)
        {
            services.AddSingleton(classType, variables);
        }

        return services;
    }

    /// <summary>
    /// Gets the list of variables missing from the provided <see cref="IConfiguration"/>. An empty list means
    /// all variables are present.
    /// </summary>
    private static List<string> GetMissingVariables(IConfiguration configuration, object variables)
    {
        List<string> missingProperties = [];
        foreach (var property in variables.GetType().GetProperties())
        {
            var attribute = property.GetCustomAttribute<EnforcedVariableAttribute>();
            if (attribute is null)
            {
                continue;
            }
            
            var variableName = attribute.VariableName ?? property.Name;
            if (configuration[variableName] is not null)
            {
                continue;
            }
            
            if (attribute.Required)
            {
                missingProperties.Add(variableName);
            }
        }

        return missingProperties;
    }
}