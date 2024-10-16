using EnforcedVariables.Exceptions;

namespace EnforcedVariables.Tests;

public class MissingVariablesExceptionTests
{
    [Test]
    public void Constructor_NoParameters_BaseMessage()
    {
        var exception = new MissingVariablesException();
        Assert.That(exception.Message, Is.EqualTo("Required variables are missing."));
    }

    [Test]
    public void Constructor_MessageProvided_MessageUtilized()
    {
        const string message = "Exception thrown.";
        var exception = new MissingVariablesException(message);
        Assert.That(exception.Message, Is.EqualTo(message));
    }
    
    [Test]
    public void Constructor_InnerExceptionProvided_MessageUtilized()
    {
        const string message = "Exception thrown.";
        var exception = new MissingVariablesException(message, new Exception());
        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.InnerException, Is.TypeOf<Exception>());
        });
    }

    [Test]
    public void Constructor_ListOfVariablesProvided_VariablesInMessage()
    {
        List<string> missingVariables = ["VariableA", "VariableB"];
        var exception = new MissingVariablesException(missingVariables);
        Assert.That(exception.Message, Is.EqualTo("The following variables are missing: VariableA, VariableB"));
    }
}