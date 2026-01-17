using System.Net;

namespace TrackForUBB.Domain.Exceptions.Custom;

public class NotFoundException(string message) : CustomException(message, HttpStatusCode.NotFound) { }