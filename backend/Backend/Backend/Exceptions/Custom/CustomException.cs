using System.Net;

namespace Backend.Exceptions.Custom;

public abstract class CustomException(string message, HttpStatusCode statusCode) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}