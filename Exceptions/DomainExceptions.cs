namespace CarRentalAPI.Exceptions;

/// <summary>
/// Bazowy wyjątek domenowy – wyrzucany gdy naruszona zostaje reguła biznesowa.
/// GlobalExceptionMiddleware zamienia go na JSON 409 Conflict.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

/// <summary>
/// Wyrzucany gdy zasób nie istnieje lub jest soft-deleted.
/// GlobalExceptionMiddleware zamienia go na JSON 404 Not Found.
/// </summary>
public class NotFoundException : Exception
{
    public string ResourceName { get; }
    public object ResourceId   { get; }

    public NotFoundException(string resourceName, object resourceId)
        : base($"{resourceName} o Id={resourceId} nie istnieje lub został usunięty.")
    {
        ResourceName = resourceName;
        ResourceId   = resourceId;
    }
}

/// <summary>
/// Wyrzucany gdy dane wejściowe są logicznie nieprawidłowe (np. złe daty).
/// GlobalExceptionMiddleware zamienia go na JSON 400 Bad Request.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
