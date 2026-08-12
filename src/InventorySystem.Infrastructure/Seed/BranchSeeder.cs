using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Infrastructure.Seed;

public static class BranchSeeder
{
    public static async Task SeedAsync(
        DbContext context,
        CancellationToken cancellationToken = default)
    {
        var branches = new List<Branch>
        {
            new()
            {
                BranchCode = "0",
                BranchNameArabic = "جميع الفروع",
                BranchNameEnglish = "All Branches",
                IsActive = true
            },
            new()
            {
                BranchCode = "001",
                BranchNameArabic = "فرع المقر",
                BranchNameEnglish = "Head Office Branch",
                IsActive = true
            },
            new()
            {
                BranchCode = "002",
                BranchNameArabic = "فرع الرجالي",
                BranchNameEnglish = "Al-Rijali Branch",
                IsActive = true
            },
            new()
            {
                BranchCode = "003",
                BranchNameArabic = "فرع الغزالية",
                BranchNameEnglish = "Ghazalia Branch",
                IsActive = true
            },
            new()
            {
                BranchCode = "004",
                BranchNameArabic = "فرع الجمعية",
                BranchNameEnglish = "Al-Jamiea Branch",
                IsActive = true
            },
            new()
            {
                BranchCode = "005",
                BranchNameArabic = "فرع حي الجامعة",
                BranchNameEnglish = "University District Branch",
                IsActive = false
            },
            new()
            {
                BranchCode = "006",
                BranchNameArabic = "فرع تكريت",
                BranchNameEnglish = "Tikrit Branch",
                IsActive = true
            },
            new()
            {
                BranchCode = "007",
                BranchNameArabic = "فرع حي الحسين",
                BranchNameEnglish = "Al-Hussein District Branch",
                IsActive = true
            },
            new()
            {
                BranchCode = "008",
                BranchNameArabic = "فرع الميكانيك",
                BranchNameEnglish = "Al-Mikanik Branch",
                IsActive = true
            },
            new()
            {
                BranchCode = "009",
                BranchNameArabic = "فرع السيدية",
                BranchNameEnglish = "Al-Saydiya Branch",
                IsActive = true
            },
            new()
            {
                BranchCode = "010",
                BranchNameArabic = "فرع حي أور",
                BranchNameEnglish = "Ur District Branch",
                IsActive = true
            },
            new()
            {
                BranchCode = "011",
                BranchNameArabic = "فرع الرمادي S20",
                BranchNameEnglish = "Ramadi S20 Branch",
                IsActive = false
            },
            new()
            {
                BranchCode = "012",
                BranchNameArabic = "فرع العامرية",
                BranchNameEnglish = "Ameriya Branch",
                IsActive = true
            },
            new()
            {
                BranchCode = "013",
                BranchNameArabic = "فرع حي الخضراء",
                BranchNameEnglish = "Al-Khadra District Branch",
                IsActive = false
            },
            new()
            {
                BranchCode = "014",
                BranchNameArabic = "فرع السماوة",
                BranchNameEnglish = "Samawah Branch",
                IsActive = false
            },
            new()
            {
                BranchCode = "015",
                BranchNameArabic = "فرع الأعظمية القديم",
                BranchNameEnglish = "Old Adhamiya Branch",
                IsActive = false
            },
            new()
            {
                BranchCode = "016",
                BranchNameArabic = "فرع الشعب",
                BranchNameEnglish = "Al-Shaab Branch",
                IsActive = true
            },
            new()
            {
                BranchCode = "017",
                BranchNameArabic = "فرع دريم ستي",
                BranchNameEnglish = "Dream City Branch",
                IsActive = false
            },
            new()
            {
                BranchCode = "018",
                BranchNameArabic = "فرع الموصل",
                BranchNameEnglish = "Mosul Branch",
                IsActive = true
            },
            new()
            {
                BranchCode = "019",
                BranchNameArabic = "فرع الشرقاط",
                BranchNameEnglish = "Al-Shirqat Branch",
                IsActive = false
            },
            new()
            {
                BranchCode = "020",
                BranchNameArabic = "فرع سامراء",
                BranchNameEnglish = "Samarra Branch",
                IsActive = true
            },
            new()
            {
                BranchCode = "021",
                BranchNameArabic = "فرع كركوك",
                BranchNameEnglish = "Kirkuk Branch",
                IsActive = true
            },
            new()
            {
                BranchCode = "022",
                BranchNameArabic = "فرع الرمادي",
                BranchNameEnglish = "Ramadi Branch",
                IsActive = true
            },
            new()
            {
                BranchCode = "023",
                BranchNameArabic = "فرع العملات",
                BranchNameEnglish = "Al-Omalaat Branch",
                IsActive = false
            },
            new()
            {
                BranchCode = "024",
                BranchNameArabic = "فرع الأعظمية الجديد",
                BranchNameEnglish = "New Adhamiya Branch",
                IsActive = true
            }
        };

        foreach (var branch in branches)
        {
            var existing = await context.Set<Branch>()
                .FirstOrDefaultAsync(
                    x => x.BranchCode == branch.BranchCode,
                    cancellationToken);

            if (existing == null)
            {
                context.Set<Branch>().Add(branch);
            }
            else
            {
                existing.BranchNameArabic = branch.BranchNameArabic;
                existing.BranchNameEnglish = branch.BranchNameEnglish;
                existing.IsActive = branch.IsActive;
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}