namespace Backend.Exceptions;

public class NotFoundException(string Message) : ExceptionResponse(System.Net.HttpStatusCode.NotFound, Message) {}