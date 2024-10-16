using EnforcedVariables.Exceptions;
using EnforcedVariables.Extensions;
using EnforcedVariables.Tests.TestHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace EnforcedVariables.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void AddEnforcedVariableClasses_VariablesFound_VariablesAddedAsSingleton(bool includeOptional)
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(true, includeOptional);
        
        Assert.DoesNotThrow(() => services.AddEnforcedVariableClasses(configuration));
        var provider = services.BuildServiceProvider();
        Assert.DoesNotThrow(() => provider.GetRequiredService<TestModel>());
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void AddVariables_VariablesFound_VariablesAddedAsSingleton(bool includeOptional)
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(true, includeOptional);
        
        Assert.DoesNotThrow(() => services.AddVariables<TestModel>(configuration));
        var provider = services.BuildServiceProvider();
        Assert.DoesNotThrow(() => provider.GetRequiredService<TestModel>());
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void AddVariables_RequiredVariablesNotFound_MissingVariablesExceptionThrown(bool includeOptional)
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(false, includeOptional);
        
        Assert.Throws<MissingVariablesException>(() => services.AddVariables<TestModel>(configuration));
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void AddVariables_DontThrowOnMissing_SingletonNotAdded(bool includeOptional)
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(false, includeOptional);
        
        Assert.DoesNotThrow(() => services.AddVariables<TestModel>(configuration, false));
        var provider = services.BuildServiceProvider();
        Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<TestModel>());
    }
}