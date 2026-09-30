using Microsoft.EntityFrameworkCore;

namespace ConduiteFacile.Demo;

public class ConduiteFacileContext : DbContext
{
    public DbSet<Moniteur> Moniteurs => Set<Moniteur>();
    public DbSet<Eleve> Eleves => Set<Eleve>();
    public DbSet<Lecon> Lecons => Set<Lecon>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=conduitefacile.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // A.2 — Nom obligatoire, 100 caractères max (Moniteur et Eleve)
        modelBuilder.Entity<Moniteur>()
            .Property(m => m.Nom)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Eleve>()
            .Property(e => e.Nom)
            .IsRequired()
            .HasMaxLength(100);

        // A.2 — Email obligatoire (Eleve)
        modelBuilder.Entity<Eleve>()
            .Property(e => e.Email)
            .IsRequired();

        // A.2 — suppression interdite (Restrict, pas de cascade) tant qu'une leçon référence
        // le moniteur ou l'élève
        modelBuilder.Entity<Lecon>()
            .HasOne(l => l.Moniteur)
            .WithMany(m => m.Lecons)
            .HasForeignKey(l => l.MoniteurId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Lecon>()
            .HasOne(l => l.Eleve)
            .WithMany(e => e.Lecons)
            .HasForeignKey(l => l.EleveId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
