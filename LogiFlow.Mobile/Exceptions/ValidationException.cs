namespace LogiFlow.Mobile.Exceptions;

/// <summary>
/// Exception thrown when user input fails validation.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="field">The field that failed validation.</param>
    /// <param name="message">The validation error message.</param>
    public ValidationException(string field, string message)
        : base(message)
    {
        Field = field;
    }

    /// <summary>
    /// Gets the field that failed validation.
    /// </summary>
    public string Field { get; }
}
