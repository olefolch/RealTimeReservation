namespace RealTimeReservation.Domain.Aggregates.Seat
{
    public class Reservation
    {
        public Guid UserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public ReservationStatus Status { get; private set; }

        private Reservation() { }

        internal Reservation(Guid userId, DateTime now, TimeSpan expiresInMinutes)
        {
            UserId = userId;
            CreatedAt = now;
            ExpiresAt = now.Add(expiresInMinutes);
            Status = ReservationStatus.Active;
        }

        public bool IsExpired(DateTime now)
        {
            return now >= ExpiresAt;
        }

        internal void Confirm()
        {
            if (Status != ReservationStatus.Active)
                throw new InvalidOperationException("Only active reservations can be confirmed.");

            Status = ReservationStatus.Confirmed;
        }

        internal void Expire()
        {
            if (Status == ReservationStatus.Confirmed)
                return;

            Status = ReservationStatus.Expired;
        }
    }
}
