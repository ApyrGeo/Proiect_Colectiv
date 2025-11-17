using System.Net;

namespace Backend.Exceptions.Custom;

public class NotFoundException(string message) : CustomException(message, HttpStatusCode.NotFound) {}