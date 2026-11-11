using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Veterinaria.Entidades;

namespace Veterinaria.Datos;

public partial class VeterinariaContext : DbContext
{
    public VeterinariaContext(DbContextOptions<VeterinariaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Mascota> Mascotas { get; set; }

    public virtual DbSet<Propietario> Propietarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Mascota>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Mascotas__3214EC073555B98C");

            entity.Property(e => e.Especie).HasMaxLength(40);
            entity.Property(e => e.Nombre).HasMaxLength(80);

            entity.HasOne(d => d.Propietario).WithMany(p => p.Mascota)
                .HasForeignKey(d => d.PropietarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mascotas_Propietarios");
        });

        modelBuilder.Entity<Propietario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Propieta__3214EC076B664327");

            entity.HasIndex(e => e.Dni, "UQ_Propietarios_Dni").IsUnique();

            entity.Property(e => e.Dni)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
