
using RealTimeReservation.Domain.Aggregates.Seat;

namespace RealTimeReservation.Application.UseCases.ConfirmSeatReservation
{
    public sealed record ConfirmSeatReservationResult(
        Guid SeatId,
        SeatStatus Status
    );
}
