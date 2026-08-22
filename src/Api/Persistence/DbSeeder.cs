
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Persistence.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        if (!await context.Ports.AnyAsync(cancellationToken))
        {
            context.Ports.AddRange(
                new Port
                {
                    Id = Guid.NewGuid(),
                    Name = "Baltra (Canal de Itabaca)",
                    Island = "Baltra / Santa Cruz",
                    Timezone = "Pacific/Galapagos"
                },
                new Port
                {
                    Id = Guid.NewGuid(),
                    Name = "Puerto Ayora",
                    Island = "Santa Cruz",
                    Timezone = "Pacific/Galapagos"
                },
                new Port
                {
                    Id = Guid.NewGuid(),
                    Name = "Puerto Baquerizo Moreno",
                    Island = "San Cristóbal",
                    Timezone = "Pacific/Galapagos"
                },
                new Port
                {
                    Id = Guid.NewGuid(),
                    Name = "Puerto Villamil",
                    Island = "Isabela",
                    Timezone = "Pacific/Galapagos"
                },
                new Port
                {
                    Id = Guid.NewGuid(),
                    Name = "Guayaquil",
                    Island = "Continente",
                    Timezone = "America/Guayaquil"
                }
            );

            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.Boats.AnyAsync(cancellationToken))
        {
            var ports = await context.Ports
                .Where(p => p.Name == "Baltra (Canal de Itabaca)" || p.Name == "Puerto Ayora")
                .Select(p => new { p.Id, p.Name })
                .ToListAsync(cancellationToken);

            var baltraPortId = ports.Single(p => p.Name == "Baltra (Canal de Itabaca)").Id;
            var ayoraPortId = ports.Single(p => p.Name == "Puerto Ayora").Id;

            context.Boats.AddRange(
                new Boat
                {
                    Id = Guid.NewGuid(),
                    Name = "Fragata",
                    BasePortId = ayoraPortId,
                    Capacity = 16
                },
                new Boat
                {
                    Id = Guid.NewGuid(),
                    Name = "Piquero Azul",
                    BasePortId = baltraPortId,
                    Capacity = 24
                }
            );

            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.Itineraries.AnyAsync(cancellationToken))
        {
            var boats = await context.Boats
                .Where(b => b.Name == "Fragata" || b.Name == "Piquero Azul")
                .Select(b => new { b.Id, b.Name })
                .ToListAsync(cancellationToken);

            var fragataId = boats.Single(b => b.Name == "Fragata").Id;
            var piqueroAzulId = boats.Single(b => b.Name == "Piquero Azul").Id;

            var ports = await context.Ports
                .Select(p => new { p.Id, p.Name })
                .ToListAsync(cancellationToken);

            var baltraPortId = ports.Single(p => p.Name == "Baltra (Canal de Itabaca)").Id;
            var ayoraPortId = ports.Single(p => p.Name == "Puerto Ayora").Id;
            var baquerizoPortId = ports.Single(p => p.Name == "Puerto Baquerizo Moreno").Id;
            var villamilPortId = ports.Single(p => p.Name == "Puerto Villamil").Id;
            var fragataItinerary = Itinerary.Create(fragataId).Value;
            var piqueroAzulItinerary = Itinerary.Create(piqueroAzulId).Value;

            context.Itineraries.AddRange(
                fragataItinerary,
                piqueroAzulItinerary);

            var galapagosOffset = TimeSpan.FromHours(-6);

            context.Segments.AddRange(
                Segment.Create(
                    fragataItinerary.Id,
                    ayoraPortId,
                    villamilPortId,
                    new DateTimeOffset(2026, 9, 14, 7, 30, 0, galapagosOffset),
                    new DateTimeOffset(2026, 9, 14, 12, 0, 0, galapagosOffset)).Value,
                Segment.Create(
                    fragataItinerary.Id,
                    villamilPortId,
                    baquerizoPortId,
                    new DateTimeOffset(2026, 9, 15, 6, 15, 0, galapagosOffset),
                    new DateTimeOffset(2026, 9, 15, 14, 15, 0, galapagosOffset)).Value,
                Segment.Create(
                    piqueroAzulItinerary.Id,
                    baltraPortId,
                    ayoraPortId,
                    new DateTimeOffset(2026, 9, 14, 17, 40, 0, galapagosOffset),
                    new DateTimeOffset(2026, 9, 14, 20, 0, 0, galapagosOffset)).Value);

            await context.SaveChangesAsync(cancellationToken);
        }

    }

}

