using Microsoft.Extensions.Configuration;

namespace EnforcedVariables.Tests.TestHelpers;

internal static class ConfigurationHelper
{
    internal static IConfiguration GetConfiguration(bool includeRequired, bool includeOptional, bool includeModel2Variables)
    {
        var environmentVariables = new Dictionary<string, string>();
        
        if (includeRequired)
        {
            environmentVariables.Add("UnNamedVariable", "UnNamedVariableValue");
            environmentVariables.Add("DifferentName1", "DifferentName1Value");
            environmentVariables.Add("Section:Variable", "SectionVariableValue");
            environmentVariables.Add("Section:InnerSection:OneMoreSection:Variable", "DeepSectionVariableValue");;
            environmentVariables.Add("IntegerValue", "42");
        }
        
        if (includeOptional)
        {
            environmentVariables.Add("NotRequiredUnNamedVariable", "NotRequiredUnNamedVariableValue");
            environmentVariables.Add("DifferentName2", "DifferentName2Value");
        }

        if (includeModel2Variables)
        {
            environmentVariables.Add("VariableEnforcedWithoutTag", "VariableEnforcedWithoutTagValue");
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(environmentVariables!)
            .Build();
    }
}