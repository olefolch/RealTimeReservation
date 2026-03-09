namespace RealTimeReservation.Application.UseCases.ReserveSeat
{
    public sealed record ReserveSeatResult(
        Guid SeatId,
        string Status
    );
}
