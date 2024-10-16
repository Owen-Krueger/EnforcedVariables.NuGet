namespace EnforcedVariables.Attributes;

/// <summary>
/// An attribute to tell a solution that an environment variable must be present and resolvable prior to
/// running the application.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class EnforcedVariableAttribute : Attribute
{
    /// <summary>
    /// The name of the variable. If null, the name is assumed to be the same as the property name.
    /// </summary>
    public string? VariableName { get; set; }

    /// <summary>
    /// Whether the variable is required prior to application startup. Defaults to true.
    /// </summary>
    public bool Required { get; set; } = true;
    
    /// <summary>
    /// Instantiates a new <see cref="EnforcedVariableAttribute"/> attribute.
    /// </summary>
    public EnforcedVariableAttribute() {}
    
    /// <summary>
    /// Instantiates a new <see cref="EnforcedVariablesAttribute"/> attribute, with the name of a variable.
    /// </summary>
    public EnforcedVariableAttribute(string variableName)
    {
        VariableName = variableName;
    }
}