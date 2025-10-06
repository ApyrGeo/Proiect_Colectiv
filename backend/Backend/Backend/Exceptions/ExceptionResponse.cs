using System.Net;

namespace Backend.Exceptions;

public class ExceptionResponse(HttpStatusCode StatusCode, string Description) : Exception
{
    public HttpStatusCode StatusCode { get; } = StatusCode;
    public string Description { get; } = Description;
}
