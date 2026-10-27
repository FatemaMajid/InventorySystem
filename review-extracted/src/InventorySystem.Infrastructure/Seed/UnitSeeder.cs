using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Infrastructure.Seed;

public static class UnitSeeder
{
    public static async Task SeedAsync(
        DbContext context,
        CancellationToken cancellationToken = default)
    {
        var units = new List<Unit>
        {
            new()
            {
                UnitCode = "1",
                UnitNameArabic = "قطعة",
                UnitNameEnglish = "Piece",
                IsActive = true
            },

            new()
            {
                UnitCode = "2",
                UnitNameArabic = "درزن",
                UnitNameEnglish = "Dozen",
                IsActive = true
            },

            new()
            {
                UnitCode = "3",
                UnitNameArabic = "سيت",
                UnitNameEnglish = "Set",
                IsActive = true
            },

            new()
            {
                UnitCode = "4",
                UnitNameArabic = "غم",
                UnitNameEnglish = "g",
                IsActive = true
            },

            new()
            {
                UnitCode = "5",
                UnitNameArabic = "كغم",
                UnitNameEnglish = "kg",
                IsActive = true
            },
            new()
            {
                UnitCode = "6",
                UnitNameArabic = "سم",
                UnitNameEnglish = "cm",
                IsActive = true
            },

            new()
            {
                UnitCode = "7",
                UnitNameArabic = "علبة",
                UnitNameEnglish = "Box",
                IsActive = true
            },

            
        };

        foreach (var unit in units)
        {
            var existing = await context.Set<Unit>()
                .FirstOrDefaultAsync(
                    x => x.UnitCode == unit.UnitCode,
                    cancellationToken);

            if (existing == null)
            {
                context.Set<Unit>().Add(unit);
            }
            else
            {
                existing.UnitNameArabic =
                    unit.UnitNameArabic;

                existing.UnitNameEnglish =
                    unit.UnitNameEnglish;

                existing.IsActive =
                    unit.IsActive;
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}