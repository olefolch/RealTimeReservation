using RealTimeReservation.Domain.Exceptions;

namespace RealTimeReservation.Domain.Aggregates.Seat
{
    public class Seat
    {
        public Guid Id { get; private set; }
        public SeatStatus Status { get; private set; }
        public Reservation? CurrentReservation { get; private set; }

        private Seat() { }

        public Seat(Guid id)
        {
            Id = id;
            Status = SeatStatus.Available;
        }

        public void Reserve(Guid userId, DateTime now, TimeSpan reservationTimeToLive)
        {
            if (Status == SeatStatus.Reserved)
                throw new SeatAlreadyReservedException();

            if (Status != SeatStatus.Available)
                throw new InvalidSeatStateException(
                    $"Cannot reserve seat when status is {Status}."
                );

            var reservation = new Reservation(userId, now, reservationTimeToLive);

            CurrentReservation = reservation;
            Status = SeatStatus.Reserved;
        }

        public void ConfirmReservation(DateTime now)
        {
            if (Status != SeatStatus.Reserved)
                throw new InvalidSeatStateException(
                    $"Cannot confirm reservation when seat status is {Status}."
                );

            if (CurrentReservation is null)
                throw new InvalidSeatStateException("No reservation to confirm.");

            if (CurrentReservation.IsExpired(now))
            {
                CurrentReservation.Expire();
                CurrentReservation = null;
                Status = SeatStatus.Available;

                throw new ReservationExpiredException();
            }

            CurrentReservation.Confirm();
            Status = SeatStatus.Confirmed;
        }

        public void ReleaseIfExpired(DateTime now)
        {
            if (CurrentReservation is null)
                return;

            if (!CurrentReservation.IsExpired(now))
                return;

            CurrentReservation.Expire();
            CurrentReservation = null;
            Status = SeatStatus.Available;
        }
    }
}
