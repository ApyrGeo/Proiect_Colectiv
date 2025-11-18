using System.Net;

namespace Domain.Exceptions.Custom;

public class NotFoundException(string message) : CustomException(message, HttpStatusCode.NotFound) {}