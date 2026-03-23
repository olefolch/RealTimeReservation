using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealTimeReservation.Domain.Aggregates.Seat;

namespace RealTimeReservation.Infrastructure.Persistence.Configurations
{
    public class SeatConfiguration : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.ToTable("Seats");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.HasOne(x => x.CurrentReservation)
                .WithOne()
                .HasForeignKey<Reservation>("SeatId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
