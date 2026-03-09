namespace RealTimeReservation.Application.UseCases.ConfirmSeatReservation
{
    public sealed record ConfirmSeatReservationCommand(
        Guid SeatId,
        DateTime Now
    );
}
