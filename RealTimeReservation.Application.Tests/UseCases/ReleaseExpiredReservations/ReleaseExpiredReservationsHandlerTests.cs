using Moq;
using RealTimeReservation.Application.Interfaces;
using RealTimeReservation.Application.UseCases.ReleaseExpiredReservations;
using RealTimeReservation.Domain.Aggregates.Seat;

namespace RealTimeReservation.Application.Tests.UseCases.ReleaseExpiredReservations
{
    public class ReleaseExpiredReservationsHandlerTests
    {
        [Fact]
        public async Task Should_Release_Expired_Seats()
        {
            var now = DateTime.UtcNow;

            var seat = new Seat(Guid.NewGuid());

            seat.Reserve(Guid.NewGuid(), now.AddMinutes(-10), TimeSpan.FromMinutes(5));

            var seats = new List<Seat> { seat };

            var repositoryMock = new Mock<ISeatRepository>();

            repositoryMock
                .Setup(x => x.GetReservedSeatsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(seats);

            var handler = new ReleaseExpiredReservationsHandler(repositoryMock.Object);

            var command = new ReleaseExpiredReservationsCommand(now);

            var result = await handler.HandleAsync(command, CancellationToken.None);

            Assert.Equal(1, result.SeatsReleased);

            repositoryMock.Verify(
                x => x.SaveAsync(seat, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Should_Not_Release_When_Reservation_Not_Expired()
        {
            var now = DateTime.UtcNow;

            var seat = new Seat(Guid.NewGuid());

            seat.Reserve(Guid.NewGuid(), now, TimeSpan.FromMinutes(5));

            var seats = new List<Seat> { seat };

            var repositoryMock = new Mock<ISeatRepository>();

            repositoryMock
                .Setup(x => x.GetReservedSeatsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(seats);

            var handler = new ReleaseExpiredReservationsHandler(repositoryMock.Object);

            var command = new ReleaseExpiredReservationsCommand(now);

            var result = await handler.HandleAsync(command, CancellationToken.None);

            Assert.Equal(0, result.SeatsReleased);

            repositoryMock.Verify(
                x => x.SaveAsync(It.IsAny<Seat>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
