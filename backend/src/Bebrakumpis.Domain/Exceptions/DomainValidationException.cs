namespace Bebrakumpis.Domain.Exceptions;

public class DomainValidationException(IEnumerable<string> errors) : Exception("Validation failed.")
{
    public IEnumerable<string> Errors { get; } = errors;
}
