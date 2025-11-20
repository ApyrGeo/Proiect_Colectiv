using FluentValidation.Results;

namespace TrackForUBB.Service.Utils;

internal class ValidationHelper
{
    internal static List<string> ConvertErrorsToListOfStrings(List<ValidationFailure> errors)
    {
        return errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
            .ToList();
    }
}
