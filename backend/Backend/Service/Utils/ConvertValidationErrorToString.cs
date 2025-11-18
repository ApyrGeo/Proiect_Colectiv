using FluentValidation.Results;

namespace Service.Utils;

internal class ConvertValidationErrorToString
{
    internal static List<string> Convert(List<ValidationFailure> errors)
    {
        return errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
            .ToList();
    }
}
