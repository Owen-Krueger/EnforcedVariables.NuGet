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
    public void AddEnforcedVariables_VariablesFound_VariablesAddedAsSingleton(bool includeOptional)
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(true, includeOptional);
        
        Assert.DoesNotThrow(() => services.AddEnforcedVariables<TestModel>(configuration));
        var provider = services.BuildServiceProvider();
        Assert.DoesNotThrow(() => provider.GetRequiredService<TestModel>());
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void AddEnforcedVariables_RequiredVariablesNotFound_MissingVariablesExceptionThrown(bool includeOptional)
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(false, includeOptional);
        
        Assert.Throws<MissingVariablesException>(() => services.AddEnforcedVariables<TestModel>(configuration, true));
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void AddEnforcedVariables_DontThrowOnMissing_SingletonNotAdded(bool includeOptional)
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(false, includeOptional);
        
        Assert.DoesNotThrow(() => services.AddEnforcedVariables<TestModel>(configuration));
        var provider = services.BuildServiceProvider();
        Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<TestModel>());
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void AddEnforcedVariables_TypeParameterVariablesFound_VariablesAddedAsSingleton(bool includeOptional)
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(true, includeOptional);
        
        Assert.DoesNotThrow(() => services.AddEnforcedVariables(configuration, typeof(TestModel)));
        var provider = services.BuildServiceProvider();
        Assert.DoesNotThrow(() => provider.GetRequiredService<TestModel>());
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void AddEnforcedVariables_TypeParameterRequiredVariablesNotFound_MissingVariablesExceptionThrown(bool includeOptional)
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(false, includeOptional);
        
        Assert.Throws<MissingVariablesException>(() => services.AddEnforcedVariables(configuration, typeof(TestModel), true));
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void AddEnforcedVariables_TypeParameterDontThrowOnMissing_SingletonNotAdded(bool includeOptional)
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(false, includeOptional);
        
        Assert.DoesNotThrow(() => services.AddEnforcedVariables(configuration, typeof(TestModel)));
        var provider = services.BuildServiceProvider();
        Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<TestModel>());
    }

    [Test]
    public void AddEnforcedVariables_NonClassTypeProvided_MissingVariablesExceptionThrown()
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(false, false);
        
        Assert.Throws<MissingVariablesException>(() => services.AddEnforcedVariables(configuration, typeof(int), true));
    }
}