# EnforcedVariables.NuGet
[![.NET](https://github.com/Owen-Krueger/EnforcedVariables.NuGet/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Owen-Krueger/EnforcedVariables.NuGet/actions/workflows/dotnet.yml)

## Summary

EnforcedVariables is a NuGet package with the goal of allowing 

This package provides attributes to decorate classes/structs and properties, as well as extension methods.

## Attributes

This package provides two attributes that can be used.

`EnforcedVariables` can be added to a class or struct to indicate that it contains variables that should be verified. Primarily used for the `IServiceCollection.AddEnforcedVariableClasses` method.

`EnforcedVariable` can be applied to a property to indicate that it represents a variable present in an `IConfiguration`.

There are two properties that can be applied to `EnforcedVariable` attributes:
- Name (string): Indicates the name of the variable within the `IConfiguration`. If not specified, the name of the property is assumed.
- Required (bool): Indicates whether the absence of the variable constitutes a catastrophic failure.

### Examples
``` C#
// Indicates this class contains variables to enforce.
[EnforcedVariables]
public class TestModel
{
    // Will check if configuration contains 'UnNamedVariable'.
    // Absence of this variable will result in an error.
    [EnforcedVariable] 
    public string UnNamedVariable { get; set; } = string.Empty;

    // Will check if configuration contains 'DifferentName1'
    // Absence of this variable will result in an error.
    [EnforcedVariable("DifferentName1")]
    public string NamedVariable { get; set; } = string.Empty;

    // Will check if configuration contains 'NotRequiredUnNamedVariable'
    // Absence of this variable will NOT result in an error.
    [EnforcedVariable(Required = false)]
    public string NotRequiredUnNamedVariable { get; set; } = string.Empty;

    // Will check if configuration contains 'DifferentName2'
    // Absence of this variable will NOT result in an error.
    [EnforcedVariable("DifferentName2", Required = false)]
    public string NotRequiredNamedVariable { get; set; } = string.Empty;

    // Will NOT check if configuration contains 'IgnoredProperty'
    // Absence of this variable will NOT result in an error.
    public string IgnoredProperty { get; set; } = string.Empty;
}
```

### IServiceCollection Extensions

This package provides a couple extensions for `IServiceCollection`: `AddEnforcedVariableClasess` and `AddEnforcedVariables`

`AddEnforcedVariableClasses`

`AddEnforcedVariables`

#### Examples

The below examples utilize the `TestModel` model above.

``` C#
var services = new ServiceCollection();
var configuration = new ConfigurationBuilder().Build();

// Adds all classes with `EnforcedVariables` attribute (TestModel).
// `TestModel` singleton will be added to `services` if all variables are present.
// No exception thrown if variables are missing.
services.AddEnforcedVariablesClasses(configuration);

// Adds all classes with `EnforcedVariables` attribute (TestModel).
// `TestModel` singleton will be added to `services` if all variables are present.
// `MissingVariablesException` exception thrown if variables are missing.
services.AddEnforcedVariablesClasses(configuration, true);
```

### IConfiguration Extensions

This package provides an `EnforceVariables` method that will verify if the `IConfiguration` contains the properties decorated with the `EnforcedVariable` attribute. If any properties are missing, a `MissingVariablesException` will be raised.

#### Examples

The below examples utilize the `TestModel` model above.

``` C#
var configuration = new ConfigurationBuilder().Build();

EnforceVariables<TestModel>(configuration);
EnforceVariables(configuration, typeof(TestModel));
```