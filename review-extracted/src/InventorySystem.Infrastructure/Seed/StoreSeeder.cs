using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Infrastructure.Seed;

public static class StoreSeeder
{
    public static async Task SeedAsync(
        DbContext context,
        CancellationToken cancellationToken = default)
    {
        var stores = new List<Store>
        {
            // =====================================================
            // 001 - فرع المقر
            // =====================================================

            new()
            {
                StoreCode = "001",
                StoreNameArabic = "مس.المخزن المقر",
                StoreNameEnglish = "Head Office Main Store",
                BranchCode = "001",
                IsActive = true
            },
            new()
            {
                StoreCode = "002",
                StoreNameArabic = "مس.الشتوي المقر",
                StoreNameEnglish = "Head Office Winter Store",
                BranchCode = "001",
                IsActive = true
            },
            new()
            {
                StoreCode = "003",
                StoreNameArabic = "مس. الصيفي المقر",
                StoreNameEnglish = "Head Office Summer Store",
                BranchCode = "001",
                IsActive = true
            },
            new()
            {
                StoreCode = "004",
                StoreNameArabic = "مس. المدرسي المقر",
                StoreNameEnglish = "Head Office School Store",
                BranchCode = "001",
                IsActive = true
            },
            new()
            {
                StoreCode = "005",
                StoreNameArabic = "مس.المفروشات المقر",
                StoreNameEnglish = "Head Office Furniture Store",
                BranchCode = "001",
                IsActive = false
            },

            // =====================================================
            // 002 - فرع الرجالي
            // =====================================================

            new()
            {
                StoreCode = "001",
                StoreNameArabic = "مس. المخزن الرجالي",
                StoreNameEnglish = "Al-Rijali Main Store",
                BranchCode = "002",
                IsActive = true
            },
            new()
            {
                StoreCode = "002",
                StoreNameArabic = "مس. الشتوي الرجالي",
                StoreNameEnglish = "Al-Rijali Winter Store",
                BranchCode = "002",
                IsActive = true
            },
            new()
            {
                StoreCode = "003",
                StoreNameArabic = "مس. الصيفي الرجالي",
                StoreNameEnglish = "Al-Rijali Summer Store",
                BranchCode = "002",
                IsActive = true
            },
            new()
            {
                StoreCode = "004",
                StoreNameArabic = "مس. المدرسي الرجالي",
                StoreNameEnglish = "Al-Rijali School Store",
                BranchCode = "002",
                IsActive = false
            },

            // =====================================================
            // 003 - فرع الغزالية
            // =====================================================

            new()
            {
                StoreCode = "001",
                StoreNameArabic = "مس.المعرض الغزالية",
                StoreNameEnglish = "Ghazalia Exhibition Store",
                BranchCode = "003",
                IsActive = true
            },
            new()
            {
                StoreCode = "002",
                StoreNameArabic = "مس.المخزن الغزالية",
                StoreNameEnglish = "Ghazalia Main Store",
                BranchCode = "003",
                IsActive = true
            },
            new()
            {
                StoreCode = "003",
                StoreNameArabic = "مس. الغزالية الشتوي",
                StoreNameEnglish = "Ghazalia Winter Store",
                BranchCode = "003",
                IsActive = true
            },
            new()
            {
                StoreCode = "004",
                StoreNameArabic = "مس. الغزالية الصيفي",
                StoreNameEnglish = "Ghazalia Summer Store",
                BranchCode = "003",
                IsActive = true
            },
            new()
            {
                StoreCode = "005",
                StoreNameArabic = "مس. الغزالية المدرسي",
                StoreNameEnglish = "Ghazalia School Store",
                BranchCode = "003",
                IsActive = true
            },

            // =====================================================
            // 004 - فرع الجمعية
            // =====================================================

            new()
            {
                StoreCode = "001",
                StoreNameArabic = "مس.الجمعية المعرض",
                StoreNameEnglish = "Al-Jamiea Exhibition Store",
                BranchCode = "004",
                IsActive = true
            },
            new()
            {
                StoreCode = "002",
                StoreNameArabic = "مس. الجمعية المخزن",
                StoreNameEnglish = "Al-Jamiea Main Store",
                BranchCode = "004",
                IsActive = true
            },
            new()
            {
                StoreCode = "003",
                StoreNameArabic = "مس. الجمعية الصيفي",
                StoreNameEnglish = "Al-Jamiea Summer Store",
                BranchCode = "004",
                IsActive = true
            },
            new()
            {
                StoreCode = "004",
                StoreNameArabic = "مس. الجمعية الشتوي",
                StoreNameEnglish = "Al-Jamiea Winter Store",
                BranchCode = "004",
                IsActive = true
            },
            new()
            {
                StoreCode = "005",
                StoreNameArabic = "مس. الجمعية المدرسي",
                StoreNameEnglish = "Al-Jamiea School Store",
                BranchCode = "004",
                IsActive = true
            },

            // =====================================================
            // 006 - فرع تكريت
            // =====================================================

            new()
            {
                StoreCode = "001",
                StoreNameArabic = "مس. تكريت المعرض",
                StoreNameEnglish = "Tikrit Exhibition Store",
                BranchCode = "006",
                IsActive = true
            },
            new()
            {
                StoreCode = "002",
                StoreNameArabic = "مس. تكريت المخزن",
                StoreNameEnglish = "Tikrit Main Store",
                BranchCode = "006",
                IsActive = true
            },
            new()
            {
                StoreCode = "003",
                StoreNameArabic = "مس. تكريت الصيفي",
                StoreNameEnglish = "Tikrit Summer Store",
                BranchCode = "006",
                IsActive = true
            },
            new()
            {
                StoreCode = "004",
                StoreNameArabic = "مس. تكريت الشتوي",
                StoreNameEnglish = "Tikrit Winter Store",
                BranchCode = "006",
                IsActive = true
            },
            new()
            {
                StoreCode = "005",
                StoreNameArabic = "مس. تكريت المدرسي",
                StoreNameEnglish = "Tikrit School Store",
                BranchCode = "006",
                IsActive = true
            },

            
            // 007 - فرع حي الحسين

            new()
            {
                StoreCode = "001",
                StoreNameArabic = "مس. حي الحسين المعرض",
                StoreNameEnglish = "Al-Hussein Exhibition Store",
                BranchCode = "007",
                IsActive = true
            },
            new()
            {
                StoreCode = "002",
                StoreNameArabic = "مس. حي الحسين المخزن",
                StoreNameEnglish = "Al-Hussein Main Store",
                BranchCode = "007",
                IsActive = true
            },
            new()
            {
                StoreCode = "003",
                StoreNameArabic = "مس. حي الحسين الصيفي",
                StoreNameEnglish = "Al-Hussein Summer Store",
                BranchCode = "007",
                IsActive = true
            },
            new()
            {
                StoreCode = "004",
                StoreNameArabic = "مس. حي الحسين الشتوي",
                StoreNameEnglish = "Al-Hussein Winter Store",
                BranchCode = "007",
                IsActive = true
            },
            new()
            {
                StoreCode = "005",
                StoreNameArabic = "مس. حي الحسين المدرسي",
                StoreNameEnglish = "Al-Hussein School Store",
                BranchCode = "007",
                IsActive = true
            },

            // =====================================================
            // 008 - فرع الميكانيك
            // =====================================================

            new()
            {
                StoreCode = "001",
                StoreNameArabic = "مس. الميكانيك المعرض",
                StoreNameEnglish = "Al-Mikanik Exhibition Store",
                BranchCode = "008",
                IsActive = true
            },
            new()
            {
                StoreCode = "002",
                StoreNameArabic = "مس. الميكانيك المخزن",
                StoreNameEnglish = "Al-Mikanik Main Store",
                BranchCode = "008",
                IsActive = true
            },
            new()
            {
                StoreCode = "003",
                StoreNameArabic = "مس. الميكانيك الصيفي",
                StoreNameEnglish = "Al-Mikanik Summer Store",
                BranchCode = "008",
                IsActive = true
            },
            new()
            {
                StoreCode = "004",
                StoreNameArabic = "مس. الميكانيك الشتوي",
                StoreNameEnglish = "Al-Mikanik Winter Store",
                BranchCode = "008",
                IsActive = true
            },
            new()
            {
                StoreCode = "005",
                StoreNameArabic = "مس. الميكانيك المدرسي",
                StoreNameEnglish = "Al-Mikanik School Store",
                BranchCode = "008",
                IsActive = true
            },

            // =====================================================
            // 009 - فرع السيدية
            // =====================================================

            new()
            {
                StoreCode = "001",
                StoreNameArabic = "مس. السيدية المعرض",
                StoreNameEnglish = "Al-Saydiya Exhibition Store",
                BranchCode = "009",
                IsActive = true
            },
            new()
            {
                StoreCode = "002",
                StoreNameArabic = "مس. السيدية المخزن",
                StoreNameEnglish = "Al-Saydiya Main Store",
                BranchCode = "009",
                IsActive = true
            },
            new()
            {
                StoreCode = "003",
                StoreNameArabic = "مس. السيدية الصيفي",
                StoreNameEnglish = "Al-Saydiya Summer Store",
                BranchCode = "009",
                IsActive = true
            },
            new()
            {
                StoreCode = "004",
                StoreNameArabic = "مس. السيدية الشتوي",
                StoreNameEnglish = "Al-Saydiya Winter Store",
                BranchCode = "009",
                IsActive = true
            },
            new()
            {
                StoreCode = "005",
                StoreNameArabic = "مس. السيدية المدرسي",
                StoreNameEnglish = "Al-Saydiya School Store",
                BranchCode = "009",
                IsActive = true
            },

            // =====================================================
            // 010 - فرع حي اور
            // =====================================================

            new()
            {
                StoreCode = "001",
                StoreNameArabic = "مس. حي اور المعرض",
                StoreNameEnglish = "Ur Exhibition Store",
                BranchCode = "010",
                IsActive = true
            },
            new()
            {
                StoreCode = "002",
                StoreNameArabic = "مس. حي اور المخزن",
                StoreNameEnglish = "Ur Main Store",
                BranchCode = "010",
                IsActive = true
            },
            new()
            {
                StoreCode = "003",
                StoreNameArabic = "مس. حي اور الصيفي",
                StoreNameEnglish = "Ur Summer Store",
                BranchCode = "010",
                IsActive = true
            },
            new()
            {
                StoreCode = "004",
                StoreNameArabic = "مس. حي اور الشتوي",
                StoreNameEnglish = "Ur Winter Store",
                BranchCode = "010",
                IsActive = true
            },
            new()
            {
                StoreCode = "005",
                StoreNameArabic = "مس. حي اور المدرسي",
                StoreNameEnglish = "Ur School Store",
                BranchCode = "010",
                IsActive = true
            },

            // =====================================================
            // 012 - فرع العامرية
            // =====================================================

            new()
            {
                StoreCode = "001",
                StoreNameArabic = "مس. العامرية المعرض",
                StoreNameEnglish = "Ameriya Exhibition Store",
                BranchCode = "012",
                IsActive = true
            },
            new()
            {
                StoreCode = "002",
                StoreNameArabic = "مس.العامرية المخزن",
                StoreNameEnglish = "Ameriya Main Store",
                BranchCode = "012",
                IsActive = true
            },
            new()
            {
                StoreCode = "003",
                StoreNameArabic = "مس. العامرية الصيفي",
                StoreNameEnglish = "Ameriya Summer Store",
                BranchCode = "012",
                IsActive = true
            },
            new()
            {
                StoreCode = "004",
                StoreNameArabic = "مس. العامرية الشتوي",
                StoreNameEnglish = "Ameriya Winter Store",
                BranchCode = "012",
                IsActive = true
            },
            new()
            {
                StoreCode = "005",
                StoreNameArabic = "مس. العامرية المدرسي",
                StoreNameEnglish = "Ameriya School Store",
                BranchCode = "012",
                IsActive = true
            },

            // =====================================================
            // 016 - فرع الشعب
            // =====================================================

            new()
            {
                StoreCode = "001",
                StoreNameArabic = "مس. الشعب المعرض",
                StoreNameEnglish = "Al-Shaab Exhibition Store",
                BranchCode = "016",
                IsActive = true
            },
            new()
            {
                StoreCode = "002",
                StoreNameArabic = "مس. الشعب المخزن",
                StoreNameEnglish = "Al-Shaab Main Store",
                BranchCode = "016",
                IsActive = true
            },
            new()
            {
                StoreCode = "003",
                StoreNameArabic = "مس. الشعب الصيفي",
                StoreNameEnglish = "Al-Shaab Summer Store",
                BranchCode = "016",
                IsActive = true
            },
            new()
            {
                StoreCode = "004",
                StoreNameArabic = "مس. الشعب الشتوي",
                StoreNameEnglish = "Al-Shaab Winter Store",
                BranchCode = "016",
                IsActive = true
            },
            new()
            {
                StoreCode = "005",
                StoreNameArabic = "مس. الشعب المدرسي",
                StoreNameEnglish = "Al-Shaab School Store",
                BranchCode = "016",
                IsActive = true
            },

            // =====================================================
            // 018 - فرع الموصل
            // =====================================================

            new()
            {
                StoreCode = "001",
                StoreNameArabic = "مس. الموصل المعرض",
                StoreNameEnglish = "Mosul Exhibition Store",
                BranchCode = "018",
                IsActive = true
            },
            new()
            {
                StoreCode = "002",
                StoreNameArabic = "مس. الموصل المخزن",
                StoreNameEnglish = "Mosul Main Store",
                BranchCode = "018",
                IsActive = true
            },
            new()
            {
                StoreCode = "003",
                StoreNameArabic = "مس. الموصل الصيفي",
                StoreNameEnglish = "Mosul Summer Store",
                BranchCode = "018",
                IsActive = true
            },
            new()
            {
                StoreCode = "004",
                StoreNameArabic = "مس. الموصل الشتوي",
                StoreNameEnglish = "Mosul Winter Store",
                BranchCode = "018",
                IsActive = true
            },
            new()
            {
                StoreCode = "005",
                StoreNameArabic = "مس. الموصل المدرسي",
                StoreNameEnglish = "Mosul School Store",
                BranchCode = "018",
                IsActive = true
            },

            // =====================================================
            // 020 - فرع سامراء
            // =====================================================

            new()
            {
                StoreCode = "001",
                StoreNameArabic = "مس. سامراء المعرض",
                StoreNameEnglish = "Samarra Exhibition Store",
                BranchCode = "020",
                IsActive = true
            },
            new()
            {
                StoreCode = "002",
                StoreNameArabic = "مس. سامراء المخزن",
                StoreNameEnglish = "Samarra Main Store",
                BranchCode = "020",
                IsActive = true
            },
            new()
            {
                StoreCode = "003",
                StoreNameArabic = "مس. سامراء الصيفي",
                StoreNameEnglish = "Samarra Summer Store",
                BranchCode = "020",
                IsActive = true
            },
            new()
            {
                StoreCode = "004",
                StoreNameArabic = "مس. سامراء الشتوي",
                StoreNameEnglish = "Samarra Winter Store",
                BranchCode = "020",
                IsActive = true
            },
            new()
            {
                StoreCode = "005",
                StoreNameArabic = "مس. سامراء المدرسي",
                StoreNameEnglish = "Samarra School Store",
                BranchCode = "020",
                IsActive = true
            },

            // =====================================================
            // 021 - فرع كركوك
            // =====================================================

            new()
            {
                StoreCode = "001",
                StoreNameArabic = "مس. كركوك المعرض",
                StoreNameEnglish = "Kirkuk Exhibition Store",
                BranchCode = "021",
                IsActive = true
            },
            new()
            {
                StoreCode = "002",
                StoreNameArabic = "مس. كركوك المخزن",
                StoreNameEnglish = "Kirkuk Main Store",
                BranchCode = "021",
                IsActive = true
            },
            new()
            {
                StoreCode = "003",
                StoreNameArabic = "مس. كركوك الصيفي",
                StoreNameEnglish = "Kirkuk Summer Store",
                BranchCode = "021",
                IsActive = true
            },
            new()
            {
                StoreCode = "004",
                StoreNameArabic = "مس. كركوك الشتوي",
                StoreNameEnglish = "Kirkuk Winter Store",
                BranchCode = "021",
                IsActive = true
            },
            new()
            {
                StoreCode = "005",
                StoreNameArabic = "مس. كركوك المدرسي",
                StoreNameEnglish = "Kirkuk School Store",
                BranchCode = "021",
                IsActive = true
            },

            // =====================================================
            // 022 - فرع الرمادي
            // =====================================================

            new()
            {
                StoreCode = "001",
                StoreNameArabic = "مس. الرمادي المعرض",
                StoreNameEnglish = "Ramadi Exhibition Store",
                BranchCode = "022",
                IsActive = true
            },
            new()
            {
                StoreCode = "002",
                StoreNameArabic = "مس. الرمادي المخزن",
                StoreNameEnglish = "Ramadi Main Store",
                BranchCode = "022",
                IsActive = true
            },
            new()
            {
                StoreCode = "003",
                StoreNameArabic = "مس. الرمادي الصيفي",
                StoreNameEnglish = "Ramadi Summer Store",
                BranchCode = "022",
                IsActive = true
            },
            new()
            {
                StoreCode = "004",
                StoreNameArabic = "مس. الرمادي الشتوي",
                StoreNameEnglish = "Ramadi Winter Store",
                BranchCode = "022",
                IsActive = true
            },
            new()
            {
                StoreCode = "005",
                StoreNameArabic = "مس. الرمادي المدرسي",
                StoreNameEnglish = "Ramadi School Store",
                BranchCode = "022",
                IsActive = true
            },

            // =====================================================
            // 024 - فرع الأعظمية الجديد
            // =====================================================

            new()
            {
                StoreCode = "001",
                StoreNameArabic = "مس. الاعظمية المعرض",
                StoreNameEnglish = "New Adhamiya Exhibition Store",
                BranchCode = "024",
                IsActive = true
            },
            new()
            {
                StoreCode = "002",
                StoreNameArabic = "مس. الاعظمية المخزن",
                StoreNameEnglish = "New Adhamiya Main Store",
                BranchCode = "024",
                IsActive = true
            },
            new()
            {
                StoreCode = "003",
                StoreNameArabic = "مس. الاعظمية الصيفي",
                StoreNameEnglish = "New Adhamiya Summer Store",
                BranchCode = "024",
                IsActive = true
            },
            new()
            {
                StoreCode = "004",
                StoreNameArabic = "مس. الاعظمية الشتوي",
                StoreNameEnglish = "New Adhamiya Winter Store",
                BranchCode = "024",
                IsActive = true
            },
            new()
            {
                StoreCode = "005",
                StoreNameArabic = "مس. الاعظمية المدرسي",
                StoreNameEnglish = "New Adhamiya School Store",
                BranchCode = "024",
                IsActive = true
            }
        };

        // =========================================================
        // StoreCode = 0
        // "جميع المستودعات" التابعة لكل فرع
        // =========================================================

        var branchCodes = stores
            .Select(x => x.BranchCode)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var branchCode in branchCodes)
        {
            var allStores = new Store
            {
                StoreCode = "0",
                StoreNameArabic = "جميع المستودعات",
                StoreNameEnglish = "All Stores",
                BranchCode = branchCode,
                IsActive = true
            };

            var existingAllStores = await context.Set<Store>()
                .FirstOrDefaultAsync(
                    x =>
                        x.StoreCode == "0" &&
                        x.BranchCode == branchCode,
                    cancellationToken);

            if (existingAllStores == null)
            {
                context.Set<Store>().Add(allStores);
            }
            else
            {
                existingAllStores.StoreNameArabic =
                    allStores.StoreNameArabic;

                existingAllStores.StoreNameEnglish =
                    allStores.StoreNameEnglish;

                existingAllStores.IsActive =
                    allStores.IsActive;
            }
        }

        // =========================================================
        // Add / Update normal stores
        // =========================================================

        foreach (var store in stores)
        {
            var existing = await context.Set<Store>()
                .FirstOrDefaultAsync(
                    x =>
                        x.StoreCode == store.StoreCode &&
                        x.BranchCode == store.BranchCode,
                    cancellationToken);

            if (existing == null)
            {
                context.Set<Store>().Add(store);
            }
            else
            {
                existing.StoreNameArabic =
                    store.StoreNameArabic;

                existing.StoreNameEnglish =
                    store.StoreNameEnglish;

                existing.IsActive =
                    store.IsActive;
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}