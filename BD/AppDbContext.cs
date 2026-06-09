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
        
        public DbSet<Person> Persons { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Documentation> Documentations { get; set; }
        public DbSet<Degree> Degrees { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        
        public DbSet<File> Files { get; set; }
        public DbSet<FileDivision> FileDivisions { get; set; }
        public DbSet<TeacherDivision> TeacherDivisions { get; set; }
        
        public DbSet<AttendanceDay> AttendanceDays { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<DivisionExam> DivisionExams { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        public DbSet<Career> Careers { get; set; }
        public DbSet<Curriculum> Curriculums { get; set; }
        public DbSet<SchoolYear> SchoolYears { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Correlative> Correlatives { get; set; }

        public DbSet<FinalExam> FinalExams { get; set; }
        public DbSet<FinalExamGrade> FinalExamGrades { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "rol-directivo-id",
                    Name = "Directivo",
                    NormalizedName = "DIRECTIVO",
                    ConcurrencyStamp = "1"
                },
                new IdentityRole{
                Name = "Preceptor",
                NormalizedName = "PRECEPTOR",
                ConcurrencyStamp = "2"
                },
                new IdentityRole{
                    Name = "Preceptor_Auxiliar",
                    NormalizedName = "PRECEPTOR_AUXILIAR",
                    ConcurrencyStamp = "3"
                },
                new IdentityRole{
                    Name = "Docente",
                    NormalizedName = "DOCENTE",
                    ConcurrencyStamp = "4"
                },
                new IdentityRole{
                    Name = "Estudiante",
                    NormalizedName = "ESTUDIANTE",
                    ConcurrencyStamp = "5"
                });

            var adminUser = new IdentityUser()
            {
                Id = "superadminISPT-2026",
                UserName = "SuperadminISPT-2026", 
                NormalizedUserName = "SUPERADMINISPT-2026",
                Email = "",
                NormalizedEmail = "",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            
            adminUser.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(adminUser, "!ISPT-AQUILES-#2026#");
            builder.Entity<IdentityUser>().HasData(adminUser);
            
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = "superadminISPT-2026",
                    RoleId = "rol-directivo-id"
                }
            );
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