using Api.DTOs;
using FluentValidation;

namespace Api.Validators;

public class CreateItineraryRequestValidator : AbstractValidator<CreateItineraryRequest>
{
    public CreateItineraryRequestValidator()
    {
        RuleFor(x => x.BoatId)
            .NotEmpty()
            .WithMessage("El identificador de la embarcación es obligatorio.")
            .WithErrorCode("EMBARCACION_REQUERIDA");
        
        RuleFor(x => x.Segments)
            .Must(segments => segments.Count >= 2)
            .WithErrorCode("MINIMO_DOS_TRAMOS")
            .WithMessage("El itinerario debe tener al menos dos tramos.");



        RuleForEach(x => x.Segments).SetValidator(new CreateSegmentRequestValidator());
    }
}
