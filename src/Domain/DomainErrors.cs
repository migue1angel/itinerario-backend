namespace Domain.Primitives;

public static class BoatErrors
{
    public static Error NotFound =>
        Error.NotFound("Boat.NotFound", "La embarcación no fue encontrada.");

    public static Error OverlappingSegments =>
        Error.Conflict(
            "Boat.OverlappingSegments",
            "La embarcación ya tiene un tramo que se solapa con el itinerario solicitado.");
}

public static class BookingErrors
{
    public static Error NotFound =>
        Error.NotFound("Booking.NotFound", "La reserva no fue encontrada.");
    public static Error ItineraryNotFound =>
        Error.NotFound("Booking.ItineraryNotFound", "El itinerario no fue encontrado.");
    public static Error CapacityExceeded =>
        Error.Conflict(
            "Booking.CapacityExceeded",
            "La reserva excede la capacidad disponible de la embarcación.");
    public static Error DepartureTooClose =>
        Error.Validation(
            "Booking.DepartureTooClose",
            "No se permiten reservas para zarpes dentro de las próximas 24 horas.");
    public static Error InvalidPassengerCount =>
        Error.Validation(
            "Booking.InvalidPassengerCount",
            "El número de pasajeros debe ser mayor a cero.");
}

public static class SegmentErrors
{
    public static Error InvalidArrivalTime =>
        Error.Validation(
            "Segment.InvalidArrivalTime",
            "La fecha de llegada debe ser posterior a la fecha de salida.");

    public static Error InvalidSegmentDuration =>
        Error.Validation(
            "Segment.InvalidSegmentDuration",
            "La duración del tramo debe ser de al menos 30 minutos y no más de 18 horas.");

    public static Error DepartureOutsideOperatingWindow(string portName) =>
        Error.Validation(
            "Segment.DepartureOutsideOperatingWindow",
            $"El zarpe desde {portName} está fuera de la ventana permitida (06:00-18:00 hora local del puerto).");

    public static Error SameDepartureAndDestinationPort =>
        Error.Validation(
            "Segment.SameDepartureAndDestinationPort",
            "El puerto de salida y el puerto de destino deben ser diferentes.");

    public static Error InvalidDepartureOffset =>
        Error.Validation(
            "Segment.InvalidDepartureOffset",
            "El offset de la fecha de zarpe no corresponde a la zona horaria del puerto de salida.");

    public static Error InvalidArrivalOffset =>
        Error.Validation(
            "Segment.InvalidArrivalOffset",
            "El offset de la fecha de arribo no corresponde a la zona horaria del puerto de destino.");
}


public static class ItineraryErrors
{
    public static Error SegmentDoesNotBelongToItinerary =>
        Error.Validation(
            "Itinerary.SegmentDoesNotBelongToItinerary",
            "El segmento no pertenece al itinerario.");

    public static Error DisconnectedRoute =>
        Error.Validation(
            "Itinerary.DisconnectedRoute",
            "El puerto de destino de un tramo debe ser el puerto de origen del siguiente.");
    public static Error NotFound =>
        Error.NotFound(
            "Itinerary.NotFound",
            "El itinerario no fue encontrado.");

    public static Error InsufficientOperationalMargin =>
        Error.Validation(
            "Itinerary.InsufficientOperationalMargin",
            "Entre el arribo de un tramo y el zarpe del siguiente deben existir al menos 45 minutos.");
}

public static class PortErrors
{
    public static Error NotFound =>
        Error.NotFound("Port.NotFound", "Uno o más puertos no fueron encontrados.");
}
