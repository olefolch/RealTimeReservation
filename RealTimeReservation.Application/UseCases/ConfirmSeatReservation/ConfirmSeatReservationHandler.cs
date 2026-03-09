using RealTimeReservation.Application.Interfaces;
using RealTimeReservation.Domain.Exceptions;

namespace RealTimeReservation.Application.UseCases.ConfirmSeatReservation
{
    public sealed class ConfirmSeatReservationHandler
    {
        private readonly ISeatRepository _seatRepository;

        public ConfirmSeatReservationHandler(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        public async Task<ConfirmSeatReservationResult> HandleAsync(
            ConfirmSeatReservationCommand command,
            CancellationToken cancellationToken)
        {
            var seat = await _seatRepository.GetByIdAsync(
                command.SeatId,
                cancellationToken);

            if (seat is null)
                throw new InvalidSeatStateException("Seat not found.");

            seat.ConfirmReservation(command.Now);

            await _seatRepository.SaveAsync(seat, cancellationToken);

            return new ConfirmSeatReservationResult(
                seat.Id,
                seat.Status);
        }
    }
}
