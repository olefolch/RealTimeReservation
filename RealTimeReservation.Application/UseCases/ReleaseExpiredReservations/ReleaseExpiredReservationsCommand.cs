namespace RealTimeReservation.Application.UseCases.ReleaseExpiredReservations
{
    public sealed record ReleaseExpiredReservationsCommand(
        DateTime Now
    );
}
