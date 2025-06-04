using System.ComponentModel;
using System.Reflection;
using EnforcedVariables.Attributes;
using EnforcedVariables.Exceptions;
using EnforcedVariables.Extensions;
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
        var variables = CreateInstance(classType, throwOnMissing);
        if (variables is null)
        {
            return services;
        }

        var missingProperties = GetMissingVariables(configuration, variables);
        
        if (throwOnMissing && missingProperties.Count > 0)
        {
            throw new MissingVariablesException(missingProperties);
        }
        
        if (addSingleton && missingProperties.Count is 0)
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
        var classAttribute = variables.GetType().GetCustomAttribute<EnforcedVariablesAttribute>();
        var enforceAllChildren = classAttribute is not null && classAttribute.EnforceAllChildren;
        
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var property in variables.GetType().GetProperties())
        {
            var attribute = property.GetCustomAttribute<EnforcedVariableAttribute>();
            if (attribute is null && !enforceAllChildren)
            {
                continue; // No need to check properties without the `EnforcedVariables` attribute.
            }
            
            var variableName = attribute?.Name ?? property.Name;
            var variableValue = GetVariableValue(variableName, configuration);
            if (ConvertType(property, variableValue, variables))
            {
                continue;
            }

            // Variable is missing or didn't resolve.
            if (attribute is null || attribute.Required)
            {
                missingProperties.Add(variableName);
            }
        }

        return missingProperties;
    }

    /// <summary>
    /// Checks if the variable exists in the configuration. If within a config section, recursively checks the section.
    /// </summary>
    private static string? GetVariableValue(string variableName, IConfiguration configuration)
    {
        var value = configuration[variableName];
        if (value is not null)
        {
            return value;
        }

        if (!variableName.Contains("__"))
        {
            return null;
        }

        // Check for config section. We force the split to only have a count of 2 to handle deep nesting.
        // For example "A__B__C" will be split into ["A", "B__C"], which would then be handled recursively.
        var parts = variableName.Split("__", 2);
        if (parts.Length < 2) // Parts should at least have the section and variable name. This should always be true.
        {
            return null;
        }

        var section = configuration.GetSection(parts[0]);

        // ReSharper disable once TailRecursiveCall
        return !section.Exists() ? null : GetVariableValue(parts[1], section);
    }

    private static bool ConvertType(PropertyInfo? property, string? variableValue, object variables)
    {
        if (variableValue is null || property is null)
        {
            return false;
        }

        try
        {
            var converter = TypeDescriptor.GetConverter(property.PropertyType);
            var convertedValue = converter.ConvertFromString(variableValue);
            property.SetValue(variables, convertedValue);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Creates an object of type <see cref="classType"/> if class or struct.
    /// If failed, throws <see cref="MissingVariablesException"/> (if `throwOnMissing` is true) or null.
    /// </summary>
    private static object? CreateInstance(Type classType, bool throwOnMissing)
    {
        switch (classType.IsClassOrStruct())
        {
            case false when throwOnMissing:
                throw new MissingVariablesException($"{classType} is not type of class or struct.");
            case false:
                return null;
        }
        
        var variables = Activator.CreateInstance(classType);

        return variables switch
        {
            null when throwOnMissing => throw new MissingVariablesException($"Failed to create {classType} type."),
            null => null,
            _ => variables
        };
    }
}