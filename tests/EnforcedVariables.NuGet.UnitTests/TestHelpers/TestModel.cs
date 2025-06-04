using EnforcedVariables.Attributes;

namespace EnforcedVariables.Tests.TestHelpers;

[EnforcedVariables]
internal class TestModel
{
    [EnforcedVariable]
    public string UnNamedVariable { get; set; } = string.Empty;

    [EnforcedVariable("DifferentName1")]
    public string NamedVariable { get; set; } = string.Empty;

    [EnforcedVariable(Required = false)]
    public string NotRequiredUnNamedVariable { get; set; } = string.Empty;

    [EnforcedVariable("DifferentName2", Required = false)]
    public string NotRequiredNamedVariable { get; set; } = string.Empty;

    [EnforcedVariable("Section__Variable")]
    public string VariableWithinSection { get; set; } = string.Empty;

    [EnforcedVariable("Section__InnerSection__OneMoreSection__Variable")]
    public string VariableWithinDeepSection { get; set; } = string.Empty;

    [EnforcedVariable]
    public int IntegerValue { get; set; }

    public string IgnoredProperty { get; set; } = string.Empty;
}

[EnforcedVariables(true)]
internal class TestModel2
{
    [EnforcedVariable("DifferentName1", Required = false)]
    public string NamedVariable { get; set; } = string.Empty;

    public string VariableEnforcedWithoutTag { get; set; } = string.Empty;

    [EnforcedVariable("Section__Variable")]
    public string VariableWithinSection { get; set; } = string.Empty;

    [EnforcedVariable("Section__InnerSection__OneMoreSection__Variable")]
    public string VariableWithinDeepSection { get; set; } = string.Empty;

    public int IntegerValue { get; set; }
}