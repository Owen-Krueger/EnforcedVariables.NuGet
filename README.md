# EnforcedVariables.NuGet
[![.NET](https://github.com/Owen-Krueger/EnforcedVariables.NuGet/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Owen-Krueger/EnforcedVariables.NuGet/actions/workflows/dotnet.yml)

## Summary

EnforcedVariables is a NuGet package with the goal of allowing consumers to ensure that a C# application has all the variables (environment, secrets, etc.) it needs prior to starting.

This package provides attributes to decorate classes/structs and properties, as well as extension methods to validate variables being present and resolvable.

## Attributes

This package provides two attributes that can be used.

`EnforcedVariables` can be added to a class or struct to indicate that it contains variables that should be verified. Primarily used for the `IServiceCollection.AddEnforcedVariableClasses` method and with health checks.

There is an optional property that can be applied to the class attribute. `EnforceAllChildren` defaults to `false`. When `true`, indicates that all properties in the class should be enforced, regardless of if they have the `EnforcedVariable` attribute or not.

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

// Indicates this class contains variables to enforce.
// The `true` provided in the constructor indicates that all properties should be
// checked, rather than only the properties with the `EnforcedVariable` attribute.
[EnforcedVariables(true)]
internal class TestModel2
{
    // Will check if configuration contains 'DifferentName1'
    // Absence of this variable will result in an error.
    [EnforcedVariable("DifferentName1", Required = false)]
    public string NamedVariable { get; set; }
    
    // Will check if configuration contains' 'VariableEnforcedWithoutTag', even without
    // `EnforcedVariable` attribute, due to class `EnforcedVariables` specifying that
    // all children should be enforced.
    // Absence of this variable will result in an error.
    public string VariableEnforcedWithoutTag { get; set; }
}
```

### IServiceCollection Extensions

This package provides a couple extensions for `IServiceCollection`: `AddEnforcedVariableClasess` and `AddEnforcedVariables`

`AddEnforcedVariableClasses` adds any classes decorated with the `EnforcedVariables` attribute to the `IServiceCollection` as a singleton. The class is only added if all variables are present and resolvable from the provided `IConfiguration`.

By default, any classes with missing variables will not throw exceptions. With any variables missing, the class won't be added to the `IServiceCollection`. Optionally, a `throwOnMissing` parameter can be provided to throw a `MissingVariablesExcetpion` if any variables are missing. 

`AddEnforcedVariables` adds the provided class type to the `IServiceCollection` as a singleton. The class is only added if all variables are present and resolvable from the provided `IConfiguration`.

By default, any classes with missing variables will not throw exceptions. With any variables missing, the class won't be added to the `IServiceCollection`. Optionally, a `throwOnMissing` parameter can be provided to throw a `MissingVariablesExcetpion` if any variables are missing.

This method can be called with a generic type or a `Type` parameter.

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

// `TestModel` singleton will be added to `services` if all variables are present.
// No exception thrown if variables are missing.
services.AddEnforcedVariables<TestModel>(configuration);

// `TestModel` singleton will be added to `services` if all variables are present.
// `MissingVariablesException` exception thrown if variables are missing.
services.AddEnforcedVariables<TestModel>(configuration, true);

// `TestModel` singleton will be added to `services` if all variables are present.
// No exception thrown if variables are missing.
// No exception will be thrown if the type is not a class/struct.
services.AddEnforcedVariables(configuration, typeof(TestModel));

// `TestModel` singleton will be added to `services` if all variables are present.
// `MissingVariablesException` exception thrown if variables are missing.
// `MissingVariablesException` will be thrown if the type is not a class/struct.
services.AddEnforcedVariables(configuration, typeof(TestModel), true);
```

### IConfiguration Extensions

This package provides an `EnforceVariables` method that will verify if the `IConfiguration` contains the properties decorated with the `EnforcedVariable` attribute. If any properties are missing, a `MissingVariablesException` will be raised.

#### Examples

The below examples utilize the `TestModel` model above.

``` C#
var configuration = new ConfigurationBuilder().Build();

// `MissingVariablesException` exception thrown if variables are missing.
EnforceVariables<TestModel>(configuration);

// `MissingVariablesException` exception thrown if variables are missing.
// `MissingVariablesException` will be thrown if the type is not a class/struct.
EnforceVariables(configuration, typeof(TestModel));
```

### EnforcedVariablesHealthCheck

The `EnforcedVariablesHealthCheck` provides an `IHealthCheck` that will indicate if any classes with the `EnforcedVariables` attribute are missing from the `IServiceProvider`. This method can be combined with `IServiceCollection.AddEnforcedVariablesClasses` to easily detect if any variables are missing that your application may depend on.

#### Examples 

``` C#
var services = new ServiceCollection();

// Adds a healthcheck to the `IServiceCollection` that will return "Unhealthy" 
// if any classes with `EnforcedVariables` attribute weren't added to the 
// `IServiceCollection` (due to missing variables).
// Uses the default health check name (enforced_variables_check).
services.AddHealthChecks()
        .AddEnforcedVariablesHealthCheck();

// Uses a different name than the default.
services.AddHealthChecks()
        .AddEnforcedVariablesHealthCheck("custom_key_name");
        
var provider = services.BuildServiceProvider();

// Stands up the healthcheck directly, rather than using the built-in health check logic.
var healthCheck = new EnforcedVariablesHealthCheck(provider);
var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());
```