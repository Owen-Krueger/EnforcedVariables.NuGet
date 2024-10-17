using Microsoft.Extensions.Configuration;

namespace EnforcedVariables.Tests.TestHelpers;

internal static class ConfigurationHelper
{
    internal static IConfiguration GetConfiguration(bool includeRequired, bool includeOptional, bool includeModel2Variables)
    {
        var environmentVariables = new Dictionary<string, string>();
        
        if (includeRequired)
        {
            environmentVariables.Add("UnNamedVariable", string.Empty);
            environmentVariables.Add("DifferentName1", string.Empty);
        }
        
        if (includeOptional)
        {
            environmentVariables.Add("NotRequiredUnNamedVariable", string.Empty);
            environmentVariables.Add("DifferentName2", string.Empty);
        }

        if (includeModel2Variables)
        {
            environmentVariables.Add("VariableEnforcedWithoutTag", string.Empty);
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(environmentVariables!)
            .Build();
    }
}