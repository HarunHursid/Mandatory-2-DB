using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Models;

public partial class DiscProfilesContext : DbContext
{
    public DiscProfilesContext()
    {
    }

    public DiscProfilesContext(DbContextOptions<DiscProfilesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<company> companies { get; set; }

    public virtual DbSet<daily_task_log> daily_task_logs { get; set; }

    public virtual DbSet<department> departments { get; set; }

    public virtual DbSet<disc_profile> disc_profiles { get; set; }

    public virtual DbSet<education> educations { get; set; }

    public virtual DbSet<employee> employees { get; set; }

    public virtual DbSet<person> persons { get; set; }

    public virtual DbSet<position> positions { get; set; }

    public virtual DbSet<project> projects { get; set; }

    public virtual DbSet<projects_disc_profile> projects_disc_profiles { get; set; }

    public virtual DbSet<social_event> social_events { get; set; }

    public virtual DbSet<stress_measure> stress_measures { get; set; }

    public virtual DbSet<task> tasks { get; set; }

    public virtual DbSet<task_evaluation> task_evaluations { get; set; }

    public virtual DbSet<vw_SocialEventsOverview> vw_SocialEventsOverviews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=HARUNPC;Initial Catalog=Disc_ProfilesDb;Integrated Security=True;Encrypt=False;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<company>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__companie__3213E83F18181406");
        });

        modelBuilder.Entity<daily_task_log>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__daily_ta__3213E83FFB8489FA");

            entity.HasOne(d => d.task).WithMany(p => p.daily_task_logs).HasConstraintName("FK_daily_task_logs_tasks");
        });

        modelBuilder.Entity<department>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__departme__3213E83FF3654105");

            entity.HasOne(d => d.company).WithMany(p => p.departments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__departmen__compa__3C69FB99");
        });

        modelBuilder.Entity<disc_profile>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__disc_pro__3213E83FF93F7293");
        });

        modelBuilder.Entity<education>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__educatio__3213E83FB0D370F4");
        });

        modelBuilder.Entity<employee>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__employee__3213E83F440470CC");

            entity.HasOne(d => d.company).WithMany(p => p.employees)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__employees__compa__48CFD27E");

            entity.HasOne(d => d.department).WithMany(p => p.employees).HasConstraintName("FK__employees__depar__4AB81AF0");

            entity.HasOne(d => d.disc_profile).WithMany(p => p.employees).HasConstraintName("FK_employees_disc_profiles");

            entity.HasOne(d => d.person).WithMany(p => p.employees).HasConstraintName("FK__employees__perso__49C3F6B7");

            entity.HasOne(d => d.position).WithMany(p => p.employees).HasConstraintName("FK_employees_positions");
        });

        modelBuilder.Entity<person>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__persons__3213E83FC3E0882B");

            entity.HasOne(d => d.Education).WithMany(p => p.people).HasConstraintName("FK_persons_educations");
        });

        modelBuilder.Entity<position>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__position__3213E83F077345D0");
        });

        modelBuilder.Entity<project>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__projects__3213E83F9736A642");

            entity.HasMany(d => d.employees).WithMany(p => p.projects)
                .UsingEntity<Dictionary<string, object>>(
                    "employees_project",
                    r => r.HasOne<employee>().WithMany()
                        .HasForeignKey("employee_id")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__employees__emplo__6383C8BA"),
                    l => l.HasOne<project>().WithMany()
                        .HasForeignKey("project_id")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__employees__proje__628FA481"),
                    j =>
                    {
                        j.HasKey("project_id", "employee_id").HasName("PK__employee__202B7EA5A8859298");
                        j.ToTable("employees_projects");
                    });
        });

        modelBuilder.Entity<projects_disc_profile>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__projects__3213E83F693B3415");

            entity.HasOne(d => d.disc_profile).WithMany(p => p.projects_disc_profiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__projects___disc___6754599E");

            entity.HasOne(d => d.project).WithMany(p => p.projects_disc_profiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__projects___proje__66603565");
        });

        modelBuilder.Entity<social_event>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__social_e__3213E83F5010B6CF");

            entity.HasOne(d => d.company).WithMany(p => p.social_events).HasConstraintName("FK__social_ev__compa__4222D4EF");

            entity.HasOne(d => d.disc_profile).WithMany(p => p.social_events).HasConstraintName("FK__social_ev__disc___412EB0B6");

            entity.HasMany(d => d.employees).WithMany(p => p.social_events)
                .UsingEntity<Dictionary<string, object>>(
                    "employees_social_event",
                    r => r.HasOne<employee>().WithMany()
                        .HasForeignKey("employee_id")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__employees__emplo__5FB337D6"),
                    l => l.HasOne<social_event>().WithMany()
                        .HasForeignKey("social_event_id")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__employees__socia__5EBF139D"),
                    j =>
                    {
                        j.HasKey("social_event_id", "employee_id").HasName("PK__employee__EF36199A85A74F95");
                        j.ToTable("employees_social_events");
                    });
        });

        modelBuilder.Entity<stress_measure>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__stress_m__3213E83F0B1358F0");

            entity.HasOne(d => d.employee).WithMany(p => p.stress_measures).HasConstraintName("FK_stress_measures_employees");

            entity.HasOne(d => d.task).WithMany(p => p.stress_measures).HasConstraintName("FK_stress_measures_tasks");
        });

        modelBuilder.Entity<task>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__tasks__3213E83FD79AA4D1");

            entity.HasOne(d => d.project).WithMany(p => p.tasks).HasConstraintName("FK__tasks__project_i__5441852A");

            entity.HasMany(d => d.employees).WithMany(p => p.tasks)
                .UsingEntity<Dictionary<string, object>>(
                    "tasks_employee",
                    r => r.HasOne<employee>().WithMany()
                        .HasForeignKey("employee_id")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__tasks_emp__emplo__5BE2A6F2"),
                    l => l.HasOne<task>().WithMany()
                        .HasForeignKey("task_id")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__tasks_emp__task___5AEE82B9"),
                    j =>
                    {
                        j.HasKey("task_id", "employee_id").HasName("PK__tasks_em__98C0F437E61D7943");
                        j.ToTable("tasks_employees");
                    });
        });

        modelBuilder.Entity<task_evaluation>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__task_eva__3213E83F2038FB3A");

            entity.HasOne(d => d.task).WithMany(p => p.task_evaluations).HasConstraintName("FK_task_evaluations_tasks");
        });

        modelBuilder.Entity<vw_SocialEventsOverview>(entity =>
        {
            entity.ToView("vw_SocialEventsOverview");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
