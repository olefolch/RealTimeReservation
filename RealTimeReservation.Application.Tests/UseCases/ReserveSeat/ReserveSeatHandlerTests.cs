using Moq;
using RealTimeReservation.Application.Interfaces;
using RealTimeReservation.Application.UseCases.ReserveSeat;
using RealTimeReservation.Domain.Aggregates.Seat;
using RealTimeReservation.Domain.Exceptions;

namespace RealTimeReservation.Application.Tests.UseCases.ReserveSeat
{
    public class ReserveSeatHandlerTests
    {
        [Fact]
        public async Task HandleAsync_ShouldReserveSeat_WhenSeatExists()
        {
            // Arrange
            var seatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var reservationTimeToLive = TimeSpan.FromMinutes(5);

            var seat = new Seat(seatId);

            var repositoryMock = new Mock<ISeatRepository>();
            repositoryMock
                .Setup(r => r.GetByIdAsync(seatId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(seat);

            var handler = new ReserveSeatHandler(repositoryMock.Object);

            var command = new ReserveSeatCommand(
                seatId,
                userId,
                now,
                reservationTimeToLive);

            // Act
            var result = await handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.Equal(seatId, result.SeatId);
            Assert.Equal("Reserved", result.Status);

            repositoryMock.Verify(
                r => r.SaveAsync(seat, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldThrow_WhenSeatDoesNotExist()
        {
            // Arrange
            var repositoryMock = new Mock<ISeatRepository>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Seat?)null);

            var handler = new ReserveSeatHandler(repositoryMock.Object);

            var command = new ReserveSeatCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow,
                TimeSpan.FromMinutes(5));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidSeatStateException>(() =>
                handler.HandleAsync(command, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_ShouldNotSave_WhenDomainThrows()
        {
            // Arrange
            var seatId = Guid.NewGuid();
            var seat = new Seat(seatId);

            var repositoryMock = new Mock<ISeatRepository>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(seatId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(seat);

            var handler = new ReserveSeatHandler(repositoryMock.Object);

            var command = new ReserveSeatCommand(
                seatId,
                Guid.NewGuid(),
                DateTime.UtcNow,
                TimeSpan.FromMinutes(5));

            // First reservation
            await handler.HandleAsync(command, CancellationToken.None);

            // Act & Assert (second reservation should fail)
            await Assert.ThrowsAsync<SeatAlreadyReservedException>(() =>
                handler.HandleAsync(command, CancellationToken.None));

            repositoryMock.Verify(
                r => r.SaveAsync(It.IsAny<Seat>(), It.IsAny<CancellationToken>()),
                Times.Once); // only first call
        }
    }
}
