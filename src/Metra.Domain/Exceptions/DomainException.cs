namespace Metra.Domain.Exceptions;

/// <summary>
/// Bazaviy domain exception
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Ma'lumot topilmagan exception
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} entity topilmadi. Key: {key}")
    {
    }
}

/// <summary>
/// Validation xatoligi exception
/// </summary>
public class ValidationException : DomainException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("Bir yoki bir nechta validation xatoliklari yuz berdi.")
    {
        Errors = errors;
    }
}

/// <summary>
/// Avtorizatsiya xatoligi exception
/// </summary>
public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "Tizimga kirish uchun avtorizatsiya talab qilinadi")
        : base(message)
    {
    }
}

/// <summary>
/// Ruxsat etilmagan amal exception
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "Bu amalni bajarish uchun ruxsat yo'q")
        : base(message)
    {
    }
}
