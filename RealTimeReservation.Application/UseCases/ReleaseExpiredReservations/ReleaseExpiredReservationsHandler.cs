using RealTimeReservation.Application.Interfaces;

namespace RealTimeReservation.Application.UseCases.ReleaseExpiredReservations
{
    public sealed class ReleaseExpiredReservationsHandler
    {
        private readonly ISeatRepository _seatRepository;

        public ReleaseExpiredReservationsHandler(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        public async Task<ReleaseExpiredReservationsResult> HandleAsync(
            ReleaseExpiredReservationsCommand command,
            CancellationToken cancellationToken)
        {
            var seats = await _seatRepository.GetReservedSeatsAsync(
                cancellationToken);

            var released = 0;

            foreach (var seat in seats)
            {
                var before = seat.Status;

                seat.ReleaseIfExpired(command.Now);

                if (before != seat.Status)
                {
                    released++;
                    await _seatRepository.SaveAsync(seat, cancellationToken);
                }
            }

            return new ReleaseExpiredReservationsResult(released);
        }
    }
}
