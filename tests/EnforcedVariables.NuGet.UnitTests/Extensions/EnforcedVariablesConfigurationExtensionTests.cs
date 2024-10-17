using EnforcedVariables.Exceptions;
using EnforcedVariables.Extensions;
using EnforcedVariables.Tests.TestHelpers;

namespace EnforcedVariables.Tests.Extensions;

public class EnforcedVariablesConfigurationExtensionTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void EnforceVariables_VariablesFound_NoExceptionThrown(bool includeOptional)
    {
        var configuration = ConfigurationHelper.GetConfiguration(true, includeOptional);
        
        Assert.DoesNotThrow(() => configuration.EnforceVariables<TestModel>());
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void EnforceVariables_RequiredVariablesNotFound_MissingVariablesExceptionThrown(bool includeOptional)
    {
        var configuration = ConfigurationHelper.GetConfiguration(false, includeOptional);
        
        Assert.Throws<MissingVariablesException>(() => configuration.EnforceVariables<TestModel>());
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void EnforceVariables_TypeParameterVariablesFound_NoExceptionThrown(bool includeOptional)
    {
        var configuration = ConfigurationHelper.GetConfiguration(true, includeOptional);
        
        Assert.DoesNotThrow(() => configuration.EnforceVariables(typeof(TestModel)));
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void EnforceVariables_TypeParameterRequiredVariablesNotFound_MissingVariablesExceptionThrown(bool includeOptional)
    {
        var configuration = ConfigurationHelper.GetConfiguration(false, includeOptional);
        
        Assert.Throws<MissingVariablesException>(() => configuration.EnforceVariables(typeof(TestModel)));
    }
}