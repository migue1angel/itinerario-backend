
using System.Globalization;
using Api.DTOs;
using FluentValidation;
using FluentValidation.Results;

namespace Api.Validators
{
    public class GetItineraryByRangeValidator : AbstractValidator<GetItineraryByRangeRequest>
    {
        public GetItineraryByRangeValidator()
        {
            RuleFor(x => x.Desde)
                .Must(value => value is null || OffsetDateTimeFormat.IsValid(value))
                .WithMessage("La fecha desde no tiene un formato válido")
                .WithErrorCode("INVALID_DATE_FORMAT");

            RuleFor(x => x.Hasta)
                .Must(value => value is null || OffsetDateTimeFormat.IsValid(value))
                .WithMessage("La fecha hasta no tiene un formato válido")
                .WithErrorCode("INVALID_DATE_FORMAT");

            RuleFor(x => x).Custom((request, validationContext) =>
            {
                if (!OffsetDateTimeFormat.IsValid(request.Desde) ||
                    !OffsetDateTimeFormat.IsValid(request.Hasta))
                    return;

                var from = DateTimeOffset.Parse(
                    request.Desde!,
                    CultureInfo.InvariantCulture);

                var to = DateTimeOffset.Parse(
                    request.Hasta!,
                    CultureInfo.InvariantCulture);

                if (to <= from)
                {
                    validationContext.AddFailure(new ValidationFailure(
                        nameof(GetItineraryByRangeRequest.Hasta),
                        "La fecha hasta debe ser posterior a la fecha desde.")
                    {
                        ErrorCode = "INVALID_DATE_RANGE"
                    });
                }
            });

        }

    }
}