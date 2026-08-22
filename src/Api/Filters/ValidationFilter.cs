
using Api.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var errors = new List<ApiError>();

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
                continue;

            var requestType = argument.GetType();

            var validatorType = typeof(IValidator<>)
                .MakeGenericType(requestType);

            var validator = context.HttpContext.RequestServices
                .GetService(validatorType) as IValidator;

            if (validator is null)
                continue;

            var validationContextType = typeof(ValidationContext<>)
                .MakeGenericType(requestType);

            var validationContext = (IValidationContext)Activator
                .CreateInstance(validationContextType, argument)!;

            var validationResult = await validator.ValidateAsync(
                validationContext,
                context.HttpContext.RequestAborted);

            errors.AddRange(validationResult.Errors.Select(
                failure => new ApiError(
                    failure.ErrorCode,
                    failure.ErrorMessage,
                    failure.PropertyName)));
        }

        if (errors.Count > 0)
        {
            context.Result = new BadRequestObjectResult(
                new ApiErrorResponse(errors));

            return;
        }

        await next();
    }
}
