using RealTimeReservation.Domain.Aggregates.Seat;

namespace RealTimeReservation.Application.Interfaces
{
    public interface ISeatRepository
    {
        Task<Seat?> GetByIdAsync(Guid seatId, CancellationToken cancellationToken);
        Task SaveAsync(Seat seat, CancellationToken cancellationToken);
        Task<IReadOnlyList<Seat>> GetReservedSeatsAsync(CancellationToken cancellationToken);
    }
}
