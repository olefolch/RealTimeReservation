namespace RealTimeReservation.Application.UseCases.ReserveSeat
{
    public sealed record ReserveSeatCommand(
        Guid SeatId,
        Guid UserId,
        DateTime Now,
        TimeSpan TimeToLive
    );
}
