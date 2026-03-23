using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealTimeReservation.Domain.Aggregates.Seat;

namespace RealTimeReservation.Infrastructure.Persistence.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();

            builder.Property(x => x.ExpiresAt).IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property<Guid>("SeatId");
        }
    }
}
