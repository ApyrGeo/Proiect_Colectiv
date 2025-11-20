using System.Net;

namespace TrackForUBB.Domain.Exceptions.Custom;

public abstract class CustomException(string message, HttpStatusCode statusCode) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}