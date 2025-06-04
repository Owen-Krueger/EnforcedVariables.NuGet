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
        var configuration = ConfigurationHelper.GetConfiguration(true, includeOptional, false);
        
        Assert.DoesNotThrow(() => services.AddEnforcedVariableClasses(configuration));
        var provider = services.BuildServiceProvider();
        Assert.DoesNotThrow(() => provider.GetRequiredService<TestModel>());
        var variables = provider.GetRequiredService<TestModel>();

        Assert.Multiple(() =>
        {
            Assert.That(variables.UnNamedVariable, Is.EqualTo("UnNamedVariableValue"));
            Assert.That(variables.NamedVariable, Is.EqualTo("DifferentName1Value"));
            Assert.That(variables.VariableWithinSection, Is.EqualTo("SectionVariableValue"));
            Assert.That(variables.VariableWithinDeepSection, Is.EqualTo("DeepSectionVariableValue"));
            Assert.That(variables.IntegerValue, Is.EqualTo(42));
        });

        AssertVariableHasCorrectValues(variables, includeOptional);
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void AddEnforcedVariables_VariablesFound_VariablesAddedAsSingleton(bool includeOptional)
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(true, includeOptional, false);
        
        Assert.DoesNotThrow(() => services.AddEnforcedVariables<TestModel>(configuration));
        var provider = services.BuildServiceProvider();
        Assert.DoesNotThrow(() => provider.GetRequiredService<TestModel>());
        var variables = provider.GetRequiredService<TestModel>();

        AssertVariableHasCorrectValues(variables, includeOptional);
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void AddEnforcedVariables_RequiredVariablesNotFound_MissingVariablesExceptionThrown(bool includeOptional)
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(false, includeOptional, false);
        
        Assert.Throws<MissingVariablesException>(() => services.AddEnforcedVariables<TestModel>(configuration, true));
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void AddEnforcedVariables_DontThrowOnMissing_SingletonNotAdded(bool includeOptional)
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(false, includeOptional, false);
        
        Assert.DoesNotThrow(() => services.AddEnforcedVariables<TestModel>(configuration));
        var provider = services.BuildServiceProvider();
        Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<TestModel>());
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void AddEnforcedVariables_TypeParameterVariablesFound_VariablesAddedAsSingleton(bool includeOptional)
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(true, includeOptional, false);
        
        Assert.DoesNotThrow(() => services.AddEnforcedVariables(configuration, typeof(TestModel)));
        var provider = services.BuildServiceProvider();
        Assert.DoesNotThrow(() => provider.GetRequiredService<TestModel>());
        var variables = provider.GetRequiredService<TestModel>();

        AssertVariableHasCorrectValues(variables, includeOptional);
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void AddEnforcedVariables_TypeParameterRequiredVariablesNotFound_MissingVariablesExceptionThrown(bool includeOptional)
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(false, includeOptional, false);
        
        Assert.Throws<MissingVariablesException>(() => services.AddEnforcedVariables(configuration, typeof(TestModel), true));
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void AddEnforcedVariables_TypeParameterDontThrowOnMissing_SingletonNotAdded(bool includeOptional)
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(false, includeOptional, false);
        
        Assert.DoesNotThrow(() => services.AddEnforcedVariables(configuration, typeof(TestModel)));
        var provider = services.BuildServiceProvider();
        Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<TestModel>());
    }

    [Test]
    public void AddEnforcedVariables_NonClassTypeProvided_MissingVariablesExceptionThrown()
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(false, false, false);
        
        Assert.Throws<MissingVariablesException>(() => services.AddEnforcedVariables(configuration, typeof(int), true));
    }

    [Test]
    public void AddEnforcedVariables_EnforceAllChildrenProvided_NonTaggedChildrenEnforced()
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(true, true, true);
        
        Assert.DoesNotThrow(() => services.AddEnforcedVariables(configuration, typeof(TestModel2)));
        var provider = services.BuildServiceProvider();
        Assert.DoesNotThrow(() => provider.GetRequiredService<TestModel2>());
        var variables = provider.GetRequiredService<TestModel2>();

        AssertVariableHasCorrectValues(variables);
    }
    
    [Test]
    public void AddEnforcedVariables_EnforceAllChildrenProvidedVariablesMissing_NonTaggedChildrenEnforced()
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(false, true, false);
        
        Assert.DoesNotThrow(() => services.AddEnforcedVariables(configuration, typeof(TestModel2)));
        var provider = services.BuildServiceProvider();
        Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<TestModel2>());
    }

    private static void AssertVariableHasCorrectValues(TestModel variables, bool includeOptional)
    {
        Assert.Multiple(() =>
        {
            Assert.That(variables.UnNamedVariable, Is.EqualTo("UnNamedVariableValue"));
            Assert.That(variables.NamedVariable, Is.EqualTo("DifferentName1Value"));
            Assert.That(variables.VariableWithinSection, Is.EqualTo("SectionVariableValue"));
            Assert.That(variables.VariableWithinDeepSection, Is.EqualTo("DeepSectionVariableValue"));
            Assert.That(variables.IntegerValue, Is.EqualTo(42));
        });

        Assert.Multiple(() =>
        {
            if (includeOptional)
            {
                Assert.That(variables.NotRequiredUnNamedVariable, Is.EqualTo("NotRequiredUnNamedVariableValue"));
                Assert.That(variables.NotRequiredNamedVariable, Is.EqualTo("DifferentName2Value"));
            }
            else
            {
                Assert.That(variables.NotRequiredUnNamedVariable, Is.EqualTo(string.Empty));
                Assert.That(variables.NotRequiredNamedVariable, Is.EqualTo(string.Empty));
            }
        });
    }

    private static void AssertVariableHasCorrectValues(TestModel2 variables)
    {
        Assert.Multiple(() =>
        {
            Assert.That(variables.NamedVariable, Is.EqualTo("DifferentName1Value"));
            Assert.That(variables.VariableWithinSection, Is.EqualTo("SectionVariableValue"));
            Assert.That(variables.VariableWithinDeepSection, Is.EqualTo("DeepSectionVariableValue"));
            Assert.That(variables.IntegerValue, Is.EqualTo(42));
        });
    }
}