using System;
using System.Collections.Generic;
using System.Text;
using BD.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using File = BD.Entidades.File;

namespace BD
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {}
        
        DbSet<Person> Persons { get; set; }
        DbSet<Location> Locations { get; set; }
        DbSet<Documentation> Documentations { get; set; }
        DbSet<Title> Titles { get; set; }
        DbSet<Contact> Contacts { get; set; }
        
        DbSet<File> Files { get; set; }
        DbSet<TeacherDivision> TeacherDivisions { get; set; }
        
        DbSet<AttendanceDay> AttendanceDays { get; set; }
        DbSet<Attendance> Attendances { get; set; }
        DbSet<Grade> Grades { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            base.Roles.AddRange(
                new IdentityRole
                {
                    Name = "Directivo",
                    NormalizedName = "DIRECTIVO"
                },
                new IdentityRole{
                Name = "Preceptor",
                NormalizedName = "PRECEPTOR"
                },
                new IdentityRole{
                    Name = "Preceptor_Auxiliar",
                    NormalizedName = "PRECEPTOR_AUXILIAR"
                },
                new IdentityRole{
                    Name = "Docente",
                    NormalizedName = "DOCENTE"
                },
                new IdentityRole{
                    Name = "Estudiante",
                    NormalizedName = "ESTUDIANTE"
                });
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