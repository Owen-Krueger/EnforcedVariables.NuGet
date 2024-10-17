namespace EnforcedVariables.Attributes;

/// <summary>
/// Indicates that the class or struct contains variables that should be enforced during startup.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class EnforcedVariablesAttribute : Attribute
{
    /// <summary>
    /// Indicates if all properties should be enforced, regardless of if they have the
    /// <see cref="EnforcedVariableAttribute"/> attribute or not.
    /// </summary>
    public bool EnforceAllChildren { get; set; } = false;
    
    /// <summary>
    /// Instantiates a new <see cref="EnforcedVariablesAttribute"/> attribute.
    /// </summary>
    public EnforcedVariablesAttribute() { }

    /// <summary>
    /// Instantiates a new <see cref="EnforcedVariablesAttribute"/> attribute, with a boolean representing if
    /// all children should be enforced, regardless of if they have the
    /// <see cref="EnforcedVariableAttribute"/> attribute or not.
    /// </summary>
    public EnforcedVariablesAttribute(bool enforceAllChildren)
    {
        EnforceAllChildren = enforceAllChildren;
    }
}