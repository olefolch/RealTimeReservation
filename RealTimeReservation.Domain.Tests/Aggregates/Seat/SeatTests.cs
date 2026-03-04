using RealTimeReservation.Domain.Aggregates.Seat;
using RealTimeReservation.Domain.Exceptions;

namespace RealTimeReservation.Domain.Tests
{
    public class SeatTests
    {
        [Fact]
        public void NewSeat_ShouldStartAsAvailable()
        {
            var seat = new Seat(Guid.NewGuid());

            Assert.Equal(SeatStatus.Available, seat.Status);
            Assert.Null(seat.CurrentReservation);
        }

        [Fact]
        public void Reserve_ShouldChangeStatusToReserved_AndCreateReservation()
        {
            var seat = new Seat(Guid.NewGuid());
            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var expiresInMinutes = TimeSpan.FromMinutes(5);

            seat.Reserve(userId, now, expiresInMinutes);

            Assert.Equal(SeatStatus.Reserved, seat.Status);
            Assert.NotNull(seat.CurrentReservation);
            Assert.Equal(userId, seat.CurrentReservation!.UserId);
            Assert.Equal(now.Add(expiresInMinutes), seat.CurrentReservation.ExpiresAt);
        }

        [Fact]
        public void Reserve_ShouldThrow_WhenSeatIsAlreadyReserved()
        {
            var seat = new Seat(Guid.NewGuid());
            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var expiresInMinutes = TimeSpan.FromMinutes(5);

            seat.Reserve(userId, now, expiresInMinutes);

            Assert.Throws<SeatAlreadyReservedException>(() =>
                seat.Reserve(Guid.NewGuid(), now, expiresInMinutes)
            );
        }

        [Fact]
        public void Reserve_ShouldThrow_WhenSeatIsConfirmed()
        {
            var seat = new Seat(Guid.NewGuid());
            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var expiresInMinutes = TimeSpan.FromMinutes(5);

            seat.Reserve(userId, now, expiresInMinutes);
            seat.ConfirmReservation(now);

            Assert.Throws<InvalidSeatStateException>(() =>
                seat.Reserve(Guid.NewGuid(), now, expiresInMinutes)
            );
        }

        [Fact]
        public void ConfirmReservation_ShouldConfirm_WhenReservationIsActive()
        {
            var seat = new Seat(Guid.NewGuid());
            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var expiresInMinutes = TimeSpan.FromMinutes(5);

            seat.Reserve(userId, now, expiresInMinutes);

            seat.ConfirmReservation(now);

            Assert.Equal(SeatStatus.Confirmed, seat.Status);
            Assert.NotNull(seat.CurrentReservation);
        }

        [Fact]
        public void ConfirmReservation_ShouldThrow_WhenSeatIsNotReserved()
        {
            var seat = new Seat(Guid.NewGuid());
            var now = DateTime.UtcNow;

            Assert.Throws<InvalidSeatStateException>(() =>
                seat.ConfirmReservation(now)
            );
        }

        [Fact]
        public void ConfirmReservation_ShouldThrow_WhenReservationIsExpired()
        {
            var seat = new Seat(Guid.NewGuid());
            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var expiresInMinutes = TimeSpan.FromMinutes(1);

            seat.Reserve(userId, now, expiresInMinutes);

            var expiredTime = now.AddMinutes(2);

            Assert.Throws<ReservationExpiredException>(() =>
                seat.ConfirmReservation(expiredTime)
            );

            Assert.Equal(SeatStatus.Available, seat.Status);
            Assert.Null(seat.CurrentReservation);
        }

        [Fact]
        public void ReleaseIfExpired_ShouldDoNothing_WhenThereIsNoReservation()
        {
            var seat = new Seat(Guid.NewGuid());
            var now = DateTime.UtcNow;

            seat.ReleaseIfExpired(now);

            Assert.Equal(SeatStatus.Available, seat.Status);
            Assert.Null(seat.CurrentReservation);
        }

        [Fact]
        public void ReleaseIfExpired_ShouldDoNothing_WhenReservationIsNotExpired()
        {
            var seat = new Seat(Guid.NewGuid());
            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var expiresInMinutes = TimeSpan.FromMinutes(10);

            seat.Reserve(userId, now, expiresInMinutes);

            seat.ReleaseIfExpired(now.AddMinutes(5));

            Assert.Equal(SeatStatus.Reserved, seat.Status);
            Assert.NotNull(seat.CurrentReservation);
        }

        [Fact]
        public void ReleaseIfExpired_ShouldReleaseSeat_WhenReservationIsExpired()
        {
            var seat = new Seat(Guid.NewGuid());
            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var expiresInMinutes = TimeSpan.FromMinutes(1);

            seat.Reserve(userId, now, expiresInMinutes);

            var expiredTime = now.AddMinutes(2);

            seat.ReleaseIfExpired(expiredTime);

            Assert.Equal(SeatStatus.Available, seat.Status);
            Assert.Null(seat.CurrentReservation);
        }
    }
}
