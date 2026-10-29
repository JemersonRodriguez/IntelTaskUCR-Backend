using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Context
{
    public class IntelTaskUCRDbContext : DbContext
    {
        public IntelTaskUCRDbContext(DbContextOptions<IntelTaskUCRDbContext> options) : base(options) { }

        public DbSet<EUsuario> T_Usuarios { get; set; } = null!;
        public DbSet<ERol> T_Roles { get; set; } = null!;
        public DbSet<ETarea> T_Tareas { get; set; } = null!;
        public DbSet<ETareaIncumplimiento> T_Tareas_Incumplimientos { get; set; } = null!;
        public DbSet<ETareaJustificacionRechazo> T_Tareas_Justificacion_Rechazo { get; set; } = null!;
        public DbSet<ETareaSeguimiento> T_Tareas_Seguimiento { get; set; } = null!; // Cambiado el nombre del DbSet a T_Tareas_Seguimiento para consistencia y añadido = null!
        public DbSet<EEstado> T_Estados { get; set; } = null!;
        public DbSet<EComplejidad> T_Complejidades { get; set; } = null!;
        public DbSet<EPrioridad> T_Prioridades { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Siempre llama al base.OnModelCreating

            // Configurar nombres de tablas
            modelBuilder.Entity<EUsuario>().ToTable("T_Usuarios");
            modelBuilder.Entity<ERol>().ToTable("T_Roles");
            modelBuilder.Entity<ETarea>().ToTable("T_Tareas");
            modelBuilder.Entity<ETareaIncumplimiento>().ToTable("T_Tareas_Incumplimientos");
            modelBuilder.Entity<ETareaJustificacionRechazo>().ToTable("T_Tareas_Justificacion_Rechazo");
            modelBuilder.Entity<ETareaSeguimiento>().ToTable("T_Tareas_Seguimiento"); // <--- ¡AÑADIDO!
            modelBuilder.Entity<EEstado>().ToTable("T_Estados");
            modelBuilder.Entity<EComplejidad>().ToTable("T_Complejidades");
            modelBuilder.Entity<EPrioridad>().ToTable("T_Prioridades");

            // Claves primarias
            modelBuilder.Entity<EUsuario>().HasKey(u => u.CN_Id_usuario);
            modelBuilder.Entity<ERol>().HasKey(r => r.CN_Id_rol);
            modelBuilder.Entity<ETarea>().HasKey(t => t.CN_Id_tarea);
            modelBuilder.Entity<ETareaIncumplimiento>().HasKey(i => i.CN_Id_tarea_incumplimiento);
            modelBuilder.Entity<ETareaJustificacionRechazo>().HasKey(j => j.CN_Id_tarea_rechazo);
            modelBuilder.Entity<ETareaSeguimiento>().HasKey(s => s.CN_Id_seguimiento); // <--- ¡AÑADIDO!
            modelBuilder.Entity<EEstado>().HasKey(e => e.CN_Id_estado);
            modelBuilder.Entity<EComplejidad>().HasKey(c => c.CN_Id_complejidad);
            modelBuilder.Entity<EPrioridad>().HasKey(p => p.CN_Id_prioridad);

            // Configurar que las PKs se generen automáticamente (IDENTITY)
            modelBuilder.Entity<ETareaIncumplimiento>()
                .Property(i => i.CN_Id_tarea_incumplimiento)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ETareaJustificacionRechazo>()
                .Property(j => j.CN_Id_tarea_rechazo)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ETareaSeguimiento>() // <--- ¡AÑADIDO!
                .Property(s => s.CN_Id_seguimiento)
                .ValueGeneratedOnAdd();

            // Relaciones

            // Usuario - Rol
            modelBuilder.Entity<EUsuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.CN_Id_rol)
                .OnDelete(DeleteBehavior.Restrict);

            // Tarea - Complejidad
            modelBuilder.Entity<ETarea>()
                .HasOne(t => t.Complejidad)
                .WithMany(c => c.Tareas)
                .HasForeignKey(t => t.CN_Id_complejidad)
                .OnDelete(DeleteBehavior.Restrict);

            // Tarea - Estado
            modelBuilder.Entity<ETarea>()
                .HasOne(t => t.Estado)
                .WithMany(e => e.Tareas)
                .HasForeignKey(t => t.CN_Id_estado)
                .OnDelete(DeleteBehavior.Restrict);

            // Tarea - Prioridad
            modelBuilder.Entity<ETarea>()
                .HasOne(t => t.Prioridad)
                .WithMany(p => p.Tareas)
                .HasForeignKey(t => t.CN_Id_prioridad)
                .OnDelete(DeleteBehavior.Restrict);

            // Tarea - Usuario Creador
            modelBuilder.Entity<ETarea>()
                .HasOne(t => t.UsuarioCreador)
                .WithMany(u => u.TareasCreadas)
                .HasForeignKey(t => t.CN_Usuario_creador)
                .OnDelete(DeleteBehavior.Restrict);

            // Tarea - Usuario Asignado
            modelBuilder.Entity<ETarea>()
                .HasOne(t => t.UsuarioAsignado)
                .WithMany(u => u.TareasAsignadas)
                .HasForeignKey(t => t.CN_Usuario_asignado)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación entre ETareaIncumplimiento y ETarea
            modelBuilder.Entity<ETareaIncumplimiento>()
                .HasOne(i => i.Tarea)
                .WithMany()
                .HasForeignKey(i => i.CN_Id_tarea)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación entre ETareaJustificacionRechazo y ETarea
            modelBuilder.Entity<ETareaJustificacionRechazo>()
                .HasOne(j => j.Tarea)
                .WithMany()
                .HasForeignKey(j => j.CN_Id_tarea)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación entre ETareaSeguimiento y ETarea <--- ¡AÑADIDO!
            modelBuilder.Entity<ETareaSeguimiento>()
                .HasOne(ts => ts.Tarea) // Un seguimiento tiene una Tarea
                .WithMany() // Una Tarea puede tener muchos seguimientos (sin colección en ETarea, por ahora)
                .HasForeignKey(ts => ts.CN_Id_tarea) // La FK es CN_Id_tarea en la tabla de seguimiento
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina la tarea, los seguimientos también
        }
    }
}