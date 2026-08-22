using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Boat> Boats => Set<Boat>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Itinerary> Itineraries => Set<Itinerary>();
    public DbSet<Port> Ports => Set<Port>();
    public DbSet<Segment> Segments => Set<Segment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Boat configuration
        modelBuilder.Entity<Boat>(builder =>
        {
            builder.HasKey(boat => boat.Id);
            builder.Property(boat => boat.Name).HasMaxLength(100).IsRequired();
            builder.HasOne(boat => boat.BasePort)
                .WithMany()
                .HasForeignKey(boat => boat.BasePortId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Booking configuration
        modelBuilder.Entity<Booking>(builder =>
        {
            builder.HasKey(booking => booking.Id);
            builder.HasOne<Itinerary>()
                .WithMany()
                .HasForeignKey(booking => booking.ItineraryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Itinerary configuration
        modelBuilder.Entity<Itinerary>(builder =>
        {
            builder.HasKey(itinerary => itinerary.Id);
            builder.HasOne(itinerary => itinerary.Boat)
                .WithMany()
                .HasForeignKey(itinerary => itinerary.BoatId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Port configuration
        modelBuilder.Entity<Port>(builder =>
        {
            builder.HasKey(port => port.Id);
        });

        // Segment configuration
        modelBuilder.Entity<Segment>(builder =>
        {
            builder.HasKey(segment => segment.Id);
            builder.Property(segment => segment.DepartureAtUtc)
                .HasColumnType("datetimeoffset(0)");
            builder.Property(segment => segment.ArrivalAtUtc)
                .HasColumnType("datetimeoffset(0)");
            builder.HasOne(segment => segment.DeparturePort)
                .WithMany()
                .HasForeignKey(segment => segment.DeparturePortId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(segment => segment.DestinationPort)
                .WithMany()
                .HasForeignKey(segment => segment.DestinationPortId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(segment => segment.Itinerary)
                .WithMany(itinerary => itinerary.Segments)
                .HasForeignKey(segment => segment.ItineraryId)
                .OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<Itinerary>()
          .Navigation(itinerary => itinerary.Segments)
          .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
