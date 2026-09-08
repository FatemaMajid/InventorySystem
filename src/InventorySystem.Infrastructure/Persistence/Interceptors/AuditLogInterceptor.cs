using Microsoft.EntityFrameworkCore.Diagnostics;

namespace InventorySystem.Infrastructure.Persistence.Interceptors;

public sealed class AuditLogInterceptor : SaveChangesInterceptor
{
}