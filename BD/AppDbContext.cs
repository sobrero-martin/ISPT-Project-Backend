using System;
using System.Collections.Generic;
using System.Text;
using BD.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BD
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        
        public override int SaveChanges()
        {
            ApplyAuditory();
            return base.SaveChanges();
        }
        
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditory();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditory()
        {
            var entidades = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseEntity &&
                            (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entrada in entidades)
            {
                var entidad = (BaseEntity) entrada.Entity;
                DateTime fechaActual = DateTime.UtcNow;

                if (entrada.State == EntityState.Added)
                {
                    entidad.CreatedAt = fechaActual;
                }
                else if (entrada.State == EntityState.Modified)
                {
                    entidad.UpdatedAt = fechaActual;
                    entrada.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                    entrada.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
                }
            }
        }
    }
}