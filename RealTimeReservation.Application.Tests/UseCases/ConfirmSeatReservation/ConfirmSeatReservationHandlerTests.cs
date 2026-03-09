using Moq;
using RealTimeReservation.Application.Interfaces;
using RealTimeReservation.Application.UseCases.ConfirmSeatReservation;
using RealTimeReservation.Domain.Aggregates.Seat;
using RealTimeReservation.Domain.Exceptions;

namespace RealTimeReservation.Application.Tests.UseCases.ConfirmSeatReservation
{
    public class ConfirmSeatReservationHandlerTests
    {
        [Fact]
        public async Task Should_Confirm_Reservation_Successfully()
        {
            var seatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var seat = new Seat(seatId);
            seat.Reserve(userId, now, TimeSpan.FromMinutes(5));

            var repositoryMock = new Mock<ISeatRepository>();

            repositoryMock
                .Setup(x => x.GetByIdAsync(seatId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(seat);

            var handler = new ConfirmSeatReservationHandler(repositoryMock.Object);

            var command = new ConfirmSeatReservationCommand(seatId, now);

            var result = await handler.HandleAsync(command, CancellationToken.None);

            Assert.Equal(seatId, result.SeatId);
            Assert.Equal(SeatStatus.Confirmed, result.Status);

            repositoryMock.Verify(
                x => x.SaveAsync(seat, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Should_Throw_When_Seat_Not_Found()
        {
            var repositoryMock = new Mock<ISeatRepository>();

            repositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Seat?)null);

            var handler = new ConfirmSeatReservationHandler(repositoryMock.Object);

            var command = new ConfirmSeatReservationCommand(
                Guid.NewGuid(),
                DateTime.UtcNow);

            await Assert.ThrowsAsync<InvalidSeatStateException>(() =>
                handler.HandleAsync(command, CancellationToken.None));
        }
    }
}
