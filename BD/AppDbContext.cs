using BD.Entidades;
using BD.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using File = BD.Entidades.File;

namespace BD
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {}
        
        public DbSet<Person> People { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Documentation> Documentations { get; set; }
        public DbSet<Degree> Degrees { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        
        public DbSet<File> Files { get; set; }
        public DbSet<FileCurriculum> FileCurriculum { get; set; }
        public DbSet<FileDivision> FileDivisions { get; set; }
        public DbSet<TeacherDivision> TeacherDivisions { get; set; }
        
        public DbSet<AttendanceDay> AttendanceDays { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<DivisionExam> DivisionExams { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<DivisionTemplate> DivisionTemplates { get; set; }
        public DbSet<ScheduleTemplate> ScheduleTemplates { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        public DbSet<Career> Careers { get; set; }
        public DbSet<Curriculum> Curriculums { get; set; }
        public DbSet<SchoolYear> SchoolYears { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Correlative> Correlatives { get; set; }

        public DbSet<FinalExam> FinalExams { get; set; }
        public DbSet<FinalExamGrade> FinalExamGrades { get; set; }
        public DbSet<FinalExamTribunal> FinalExamTribunals { get; set; }
        public DbSet<News> News { get; set; }

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
                Id = "4d8d1bdb-d4aa-4a55-b011-1b5d40695275",
                Name = "Preceptor",
                NormalizedName = "PRECEPTOR",
                ConcurrencyStamp = "2"
                },
                new IdentityRole{
                    Id = "0ad3e500-ba7b-4df4-b2cb-6f3326f85804",
                    Name = "Preceptor_Auxiliar",
                    NormalizedName = "PRECEPTOR_AUXILIAR",
                    ConcurrencyStamp = "3"
                },
                new IdentityRole{
                    Id = "a54945c5-a4b1-48fe-b95e-50d083129604",
                    Name = "Docente",
                    NormalizedName = "DOCENTE",
                    ConcurrencyStamp = "4"
                },
                new IdentityRole{
                    Id = "7c8ecc3e-d4e7-4585-95d0-9fda2467dcc4",
                    Name = "Estudiante",
                    NormalizedName = "ESTUDIANTE",
                    ConcurrencyStamp = "5"
                });

            string superadminId = "ae65a54a-aab5-4d0c-aa17-554334b49e8c";
            var adminUser = new IdentityUser()
            {
                Id = superadminId,
                UserName = "SuperadminISPT-2026", 
                NormalizedUserName = "SUPERADMINISPT-2026",
                Email = "",
                NormalizedEmail = "",
                EmailConfirmed = true,
                SecurityStamp = "8f1603ac-e108-4772-9edb-821fe0acba81",
                ConcurrencyStamp = "1f5c45eb-b299-4081-9544-343297c3c9ac"
            };

            adminUser.PasswordHash = "AQAAAAIAAYagAAAAENwl/PAOUW6XLPdWM3XDdIyTF+kAIvEN09qqM50iXH7Rl8f21BVn0J0hV813DSA8dQ==";
            builder.Entity<IdentityUser>().HasData(adminUser);
            
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = superadminId,
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
