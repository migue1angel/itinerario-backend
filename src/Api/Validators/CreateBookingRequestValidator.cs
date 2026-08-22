using Api.DTOs;
using FluentValidation;

namespace Api.Validators;

public class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.PassengerCount)
            .GreaterThan(0)
            .WithMessage("El número de pasajeros debe ser mayor que cero.")
            .WithErrorCode("NUMERO_PASAJEROS_INVALIDO");
    }
}
