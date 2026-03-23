using Microsoft.EntityFrameworkCore;
using RealTimeReservation.Application.Interfaces;
using RealTimeReservation.Domain.Aggregates.Seat;

namespace RealTimeReservation.Infrastructure.Persistence.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly AppDbContext _context;

        public SeatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Seat?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _context.Seats
                .Include(x => x.CurrentReservation)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Seat>> GetReservedSeatsAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Seats
                .Include(x => x.CurrentReservation)
                .Where(x => x.Status == SeatStatus.Reserved)
                .ToListAsync(cancellationToken);
        }

        public async Task SaveAsync(
            Seat seat,
            CancellationToken cancellationToken)
        {
            var tracked = await _context.Seats
                .AnyAsync(x => x.Id == seat.Id, cancellationToken);

            if (!tracked)
                _context.Seats.Add(seat);
            else
                _context.Seats.Update(seat);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
