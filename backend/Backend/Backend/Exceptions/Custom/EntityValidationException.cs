using FluentValidation.Results;
using System.Net;

namespace Backend.Exceptions.Custom;

public class EntityValidationException : CustomException
{
    public EntityValidationException(string message)
        : base(message, HttpStatusCode.UnprocessableEntity){}

    public EntityValidationException(List<ValidationFailure> errors)
        : base(string.Join(" ", errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")), 
            HttpStatusCode.UnprocessableEntity){}
}
