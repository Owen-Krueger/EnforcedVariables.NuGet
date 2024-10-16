namespace EnforcedVariables.Attributes;

/// <summary>
/// Indicates that the class or struct contains variables that should be enforced during startup.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class EnforcedVariablesAttribute : Attribute;