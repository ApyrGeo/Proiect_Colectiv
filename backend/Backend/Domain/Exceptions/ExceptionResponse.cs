using System.Net;

namespace Domain.Exceptions;

public class ExceptionResponse(HttpStatusCode statusCode, string description)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
    public string Description { get; } = description;
}
