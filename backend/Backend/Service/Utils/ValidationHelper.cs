using FluentValidation.Results;

namespace TrackForUBB.Service.Utils;

internal class ValidationHelper
{
    internal static List<string> ConvertErrorToListOfStrings(List<ValidationFailure> errors)
    {
        return errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
            .ToList();
    }
}
