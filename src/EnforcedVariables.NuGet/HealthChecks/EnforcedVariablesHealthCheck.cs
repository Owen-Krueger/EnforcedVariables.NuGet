using System.Reflection;
using EnforcedVariables.Attributes;
using EnforcedVariables.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EnforcedVariables.HealthChecks;

/// <summary>
/// Used for building health checks around enforced variables.
/// </summary>
public static class EnforcedVariablesHealthCheckBuilder
{
    /// <summary>
    /// The default name used for a health check around enforced variables.
    /// </summary>
    public const string DefaultName = "enforced_variables_check";

    /// <summary>
    /// Adds a health check around enforcing variables to the provided <see cref="IHealthChecksBuilder"/>.
    /// </summary>
    /// <param name="builder">A builder used to register health checks.</param>
    /// <param name="name">The health check name.</param>
    /// <param name="failureStatus">
    /// The <see cref="HealthStatus"/> that should be reported when the health check reports a failure. If the provided value
    /// is <c>null</c>, then <see cref="HealthStatus.Unhealthy"/> will be reported.
    /// </param>
    /// <param name="tags">A list of tags that can be used for filtering health checks.</param>
    public static IHealthChecksBuilder AddEnforcedVariablesHealthCheck(
        this IHealthChecksBuilder builder,
        string? name = null,
        HealthStatus failureStatus = default,
        IEnumerable<string>? tags = default)
    {
        return builder.Add(new HealthCheckRegistration(
                name ?? DefaultName,
                sp => new EnforcedVariablesHealthCheck(sp),
            failureStatus,
            tags));
    }
}

/// <summary>
/// For performing health checks on classes with variables that should be enforced (Decorated with the
/// <see cref="EnforcedVariablesAttribute"/> attribute). Checks if any environment variables are missing that are
/// necessary to run the application.
/// </summary>
public class EnforcedVariablesHealthCheck(IServiceProvider serviceProvider) : IHealthCheck
{
    /// <summary>
    /// Runs the health check, returning the status of the component being checked. Checks if any environment
    /// variables are missing that are necessary to run the application.
    /// </summary>
    /// <param name="context">A context object associated with the current execution.</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> that can be used to cancel the health check.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that completes when the health check has finished, yielding the status of the component being checked.</returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        var assembly = Assembly.GetCallingAssembly();
        var missingClasses = assembly.GetEnforcedVariablesClasses()
            .Where(type => serviceProvider.GetService(type) is null)
            .ToList();
        
        return Task.FromResult(missingClasses.Count == 0 ? 
            HealthCheckResult.Healthy("Variables present.") :
            HealthCheckResult.Unhealthy($"Following classes are not present in ServiceProvider: {string.Join(',', missingClasses.Select(x => x.Name))}"));

    }
}