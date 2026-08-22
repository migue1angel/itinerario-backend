using System.Globalization;
using System.Text.RegularExpressions;
using Api.DTOs;
using FluentValidation;

namespace Api.Validators;

public class CreateSegmentRequestValidator : AbstractValidator<CreateSegmentRequest>
{

    public CreateSegmentRequestValidator()
    {
        RuleFor(x => x.DeparturePortId).NotEmpty()
            .WithMessage("El puerto de salida es obligatorio.")
            .WithErrorCode("PUERTO_SALIDA_REQUERIDO");

        RuleFor(x => x.DestinationPortId).NotEmpty()
            .WithMessage("El puerto de destino es obligatorio.")
            .WithErrorCode("PUERTO_DESTINO_REQUERIDO");

        RuleFor(x => x.DepartureAt)
            .Must(OffsetDateTimeFormat.IsValid)
            .WithMessage("El zarpe debe incluir el desplazamiento (offset) explícito.")
            .WithErrorCode("FECHA_INVALIDA");

        RuleFor(x => x.ArrivalAt)
            .Must(OffsetDateTimeFormat.IsValid)
            .WithMessage("El arribo debe incluir el desplazamiento (offset) explícito.")
            .WithErrorCode("FECHA_INVALIDA");
    }


}
