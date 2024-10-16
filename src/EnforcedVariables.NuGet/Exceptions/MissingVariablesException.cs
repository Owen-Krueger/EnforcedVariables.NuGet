namespace EnforcedVariables.Exceptions;

/// <summary>
/// An exception thrown when required variables are missing while standing up the application.
/// </summary>
[Serializable]
public class MissingVariablesException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingVariablesException"/> class.
    /// </summary>
    public MissingVariablesException() : base("Required variables are missing.") { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingVariablesException"/> class with a specified error message.
    /// </summary>
    public MissingVariablesException(string message) : base(message) { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingVariablesException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    public MissingVariablesException(string message, Exception inner) : base(message, inner) { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingVariablesException"/> class with a list of missing
    /// variables, used to format the error message.
    /// </summary>
    public MissingVariablesException(List<string> missingVariables)
        : base($"The following variables are missing: {string.Join(", ", missingVariables)}") { }
}