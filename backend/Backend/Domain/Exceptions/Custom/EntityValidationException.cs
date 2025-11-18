using System.Net;

namespace Domain.Exceptions.Custom;

public class EntityValidationException : CustomException
{
    public EntityValidationException(string message)
        : base(message, HttpStatusCode.UnprocessableEntity){}

    public EntityValidationException(List<string> errors)
        : base(string.Join(" ", errors), HttpStatusCode.UnprocessableEntity){}
}
