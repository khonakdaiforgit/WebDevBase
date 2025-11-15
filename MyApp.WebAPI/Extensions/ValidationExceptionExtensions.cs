using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyApp.Infrastructure.Common.Exceptions;

namespace MyApp.WebAPI.Extensions;

public static class ValidationExceptionExtensions
{
    public static ValidationProblemDetails ToValidationProblemDetails(this ValidationException ex)
    {
        var modelState = new ModelStateDictionary();
        foreach (var (key, errors) in ex.Errors)
        {
            foreach (var error in errors)
            {
                modelState.AddModelError(key, error);
            }
        }
        return new ValidationProblemDetails(modelState);
    }
}