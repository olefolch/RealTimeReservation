using RealTimeReservation.Application.Interfaces;
using RealTimeReservation.Domain.Exceptions;

namespace RealTimeReservation.Application.UseCases.ReserveSeat
{
    public sealed class ReserveSeatHandler
    {
        private readonly ISeatRepository _seatRepository;

        public ReserveSeatHandler(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        public async Task<ReserveSeatResult> HandleAsync(
            ReserveSeatCommand command,
            CancellationToken cancellationToken)
        {
            var seat = await _seatRepository.GetByIdAsync(
                command.SeatId,
                cancellationToken);

            if (seat is null)
                throw new InvalidSeatStateException("Seat not found.");

            seat.Reserve(
                command.UserId,
                command.Now,
                command.TimeToLive);

            await _seatRepository.SaveAsync(seat, cancellationToken);

            return new ReserveSeatResult(
                seat.Id,
                seat.Status.ToString());
        }
    }
}
