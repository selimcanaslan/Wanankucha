using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Wanankucha.Domain.Common;
using Wanankucha.Domain.Entities.Identity;

namespace Wanankucha.Persistence.Contexts;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, Guid>(options)
{
    // Kendi entity'lerimizi buraya DbSet olarak ekleyeceğiz.
    // public DbSet<Product> Products { get; set; } // Örnek

    // SaveChangesAsync metodunu eziyoruz (Override).
    // Böylece her kayıt işleminde araya girip tarihleri set edebileceğiz.
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // ChangeTracker: Entity'ler üzerindeki değişiklikleri takip eden mekanizma.
        var data = ChangeTracker.Entries<BaseEntity<Guid>>();

        foreach (var d in data)
        {
            switch (d.State) // İşlemin türüne göre (Ekleme mi, Güncelleme mi?)
            {
                case EntityState.Added:
                    d.Entity.CreatedDate = DateTime.UtcNow; // Postgres UTC sever
                    break;
                case EntityState.Modified:
                    d.Entity.UpdatedDate = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}