using EnforcedVariables.Extensions;
using EnforcedVariables.HealthChecks;
using EnforcedVariables.Tests.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EnforcedVariables.Tests;

public class EnforcedVariablesHealthCheckTests
{
    [Test]
    public async Task EnforcedVariablesHealthCheck_VariablesPresent_Healthy()
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(true, true);
        services.AddEnforcedVariableClasses(configuration);
        var provider = services.BuildServiceProvider();

        var healthCheck = new EnforcedVariablesHealthCheck(provider);
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());
        
        Assert.That(result.Status, Is.EqualTo(HealthStatus.Healthy));
    }
    
    [Test]
    public async Task EnforcedVariablesHealthCheck_VariablesMissing_Unhealthy()
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.GetConfiguration(false, false);
        services.AddEnforcedVariableClasses(configuration);
        var provider = services.BuildServiceProvider();

        var healthCheck = new EnforcedVariablesHealthCheck(provider);
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());
        
        Assert.That(result.Status, Is.EqualTo(HealthStatus.Unhealthy));
    }
}