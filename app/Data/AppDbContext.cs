using Microsoft.EntityFrameworkCore;
using MiApi.Entities;

namespace MiApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets para todas las entidades
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<SeccionAlmacenamiento> Secciones => Set<SeccionAlmacenamiento>();
    public DbSet<TipoCafe> TiposCafe => Set<TipoCafe>();
    public DbSet<Inventario> Inventarios => Set<Inventario>();
    public DbSet<MovimientoEntrada> MovimientosEntrada => Set<MovimientoEntrada>();
    public DbSet<MovimientoSalida> MovimientosSalida => Set<MovimientoSalida>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- AdminUser ----------
        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
        });

        // ---------- Cliente ----------
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasIndex(c => c.Nombre).IsUnique();
        });

        // ---------- SeccionAlmacenamiento ----------
        modelBuilder.Entity<SeccionAlmacenamiento>(entity =>
        {
            entity.HasIndex(s => s.Nombre).IsUnique();
        });

        // ---------- TipoCafe ----------
        modelBuilder.Entity<TipoCafe>(entity =>
        {
            entity.HasIndex(t => t.Nombre).IsUnique();
        });

        // ---------- Inventario ----------
        modelBuilder.Entity<Inventario>(entity =>
        {
            // Clave única compuesta: no puede repetirse la misma combinación Sección/TipoCafé
            entity.HasIndex(i => new { i.SeccionId, i.TipoCafeId }).IsUnique();

            // Relaciones
            entity.HasOne(i => i.Seccion)
                  .WithMany()
                  .HasForeignKey(i => i.SeccionId)
                  .OnDelete(DeleteBehavior.Restrict); // No eliminar sección si hay inventario

            entity.HasOne(i => i.TipoCafe)
                  .WithMany()
                  .HasForeignKey(i => i.TipoCafeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- MovimientoEntrada ----------
        modelBuilder.Entity<MovimientoEntrada>(entity =>
        {
            entity.HasOne(m => m.Cliente)
                  .WithMany()
                  .HasForeignKey(m => m.ClienteId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Seccion)
                  .WithMany()
                  .HasForeignKey(m => m.SeccionId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.TipoCafe)
                  .WithMany()
                  .HasForeignKey(m => m.TipoCafeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.AdminUser)
                  .WithMany()
                  .HasForeignKey(m => m.AdminUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- MovimientoSalida ----------
        modelBuilder.Entity<MovimientoSalida>(entity =>
        {
            entity.HasOne(m => m.Cliente)
                  .WithMany()
                  .HasForeignKey(m => m.ClienteId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Seccion)
                  .WithMany()
                  .HasForeignKey(m => m.SeccionId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.TipoCafe)
                  .WithMany()
                  .HasForeignKey(m => m.TipoCafeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.AdminUser)
                  .WithMany()
                  .HasForeignKey(m => m.AdminUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}