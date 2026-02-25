using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ONTHERUSH.AccesoADatos.Models;

namespace ONTHERUSH.AccesoADatos.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Conductor> Conductores { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Viaje> Viajes { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }



        //Tal vez esto se tenga que cambiar 
        public DbSet<SolicitudCambio> SolicitudesCambio { get; set; }
        //

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Viaje>()
                .HasOne(v => v.Conductor)
                .WithMany(c => c.Viajes)
                .HasForeignKey(v => v.ConductorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Viaje>()
                .HasOne(v => v.Vehiculo)
                .WithMany(vh => vh.Viajes)
                .HasForeignKey(v => v.VehiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Viaje>()
                .HasOne(v => v.AsignadoPor)
                .WithMany()
                .HasForeignKey(v => v.AsignadoPorAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Reserva>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}