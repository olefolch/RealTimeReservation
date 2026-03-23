using Microsoft.EntityFrameworkCore;
using RealTimeReservation.Domain.Aggregates.Seat;

namespace RealTimeReservation.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Seat> Seats => Set<Seat>();
        public DbSet<Reservation> Reservations => Set<Reservation>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
