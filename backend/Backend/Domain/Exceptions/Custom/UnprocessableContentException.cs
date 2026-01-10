using System.Net;

namespace TrackForUBB.Domain.Exceptions.Custom;

public class UnprocessableContentException(string message) : CustomException(message, HttpStatusCode.UnprocessableContent) { }
