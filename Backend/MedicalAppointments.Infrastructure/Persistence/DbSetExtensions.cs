using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Infrastructure.Persistence;

internal static class DbSetExtensions
{
    public static void UpdateIfDetached<TEntity>(this DbSet<TEntity> set, TEntity entity)
        where TEntity : class
    {
        if (set.Entry(entity).State == EntityState.Detached)
        {
            set.Update(entity);
        }
    }
}
