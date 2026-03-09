namespace RealTimeReservation.Application.UseCases.ReleaseExpiredReservations
{
    public sealed record ReleaseExpiredReservationsResult(
        int SeatsReleased
    );
}
