using Microsoft.EntityFrameworkCore;
using ConduiteFacile.Demo;

// =====================================================================
// SOLUTION — Challenge de fin de Module 2 : ConduiteFacile
// Document réservé au formateur : ne pas diffuser avant la fin de l'épreuve.
// =====================================================================

using var context = new ConduiteFacileContext();

// ---------------------------------------------------------------------
// Partie A — Mise en place et modélisation
// ---------------------------------------------------------------------

// A.3 — migration en attente : voir README.md (dotnet ef migrations add InitConduiteFacile
// puis dotnet ef database update), non reproduite ici (artefact généré, pas du code écrit à la main)
await context.Database.MigrateAsync();

// A.4 — insertion automatique du jeu de données fourni par l'énoncé, seulement si la base est vide
if (!await context.Moniteurs.AnyAsync())
{
    var nathalie = new Moniteur { Nom = "Nathalie Moreau", NumeroPermis = "PL-4471" };
    var julien = new Moniteur { Nom = "Julien Lefevre", NumeroPermis = "PL-2290" };
    var thomas = new Moniteur { Nom = "Thomas Girard", NumeroPermis = "PL-5588" };
    context.Moniteurs.AddRange(nathalie, julien, thomas);

    var nicolas = new Eleve { Nom = "Nicolas Dubois", Email = "nicolas.dubois@mail.test", DateInscription = new DateTime(2025, 1, 10) };
    var camille = new Eleve { Nom = "Camille Lambert", Email = "camille.lambert@mail.test", DateInscription = new DateTime(2025, 1, 15) };
    var antoine = new Eleve { Nom = "Antoine Mercier", Email = "antoine.mercier@mail.test", DateInscription = new DateTime(2025, 2, 2) };
    var lucie = new Eleve { Nom = "Lucie Rousseau", Email = "lucie.rousseau@mail.test", DateInscription = new DateTime(2025, 3, 20) };
    context.Eleves.AddRange(nicolas, camille, antoine, lucie);

    context.Lecons.AddRange(
        new Lecon { DateHeure = new DateTime(2025, 5, 5, 9, 0, 0), DureeMinutes = 60, Statut = StatutLecon.Realisee, Moniteur = nathalie, Eleve = nicolas },
        new Lecon { DateHeure = new DateTime(2025, 5, 5, 10, 30, 0), DureeMinutes = 45, Statut = StatutLecon.Realisee, Moniteur = nathalie, Eleve = camille },
        new Lecon { DateHeure = new DateTime(2025, 5, 6, 9, 0, 0), DureeMinutes = 60, Statut = StatutLecon.Realisee, Moniteur = julien, Eleve = nicolas },
        new Lecon { DateHeure = new DateTime(2025, 5, 7, 14, 0, 0), DureeMinutes = 90, Statut = StatutLecon.Annulee, Moniteur = julien, Eleve = antoine },
        new Lecon { DateHeure = new DateTime(2025, 5, 8, 9, 0, 0), DureeMinutes = 60, Statut = StatutLecon.Planifiee, Moniteur = nathalie, Eleve = lucie },
        new Lecon { DateHeure = new DateTime(2025, 5, 8, 11, 0, 0), DureeMinutes = 60, Statut = StatutLecon.Planifiee, Moniteur = thomas, Eleve = camille },
        new Lecon { DateHeure = new DateTime(2025, 5, 9, 9, 0, 0), DureeMinutes = 45, Statut = StatutLecon.Planifiee, Moniteur = nathalie, Eleve = nicolas },
        new Lecon { DateHeure = new DateTime(2025, 5, 10, 9, 0, 0), DureeMinutes = 60, Statut = StatutLecon.Planifiee, Moniteur = julien, Eleve = lucie },
        new Lecon { DateHeure = new DateTime(2025, 5, 11, 9, 0, 0), DureeMinutes = 30, Statut = StatutLecon.Planifiee, Moniteur = thomas, Eleve = lucie }
    );

    await context.SaveChangesAsync();
    Console.WriteLine("Jeu de données inséré.\n");
}

var reference = new DateTime(2025, 5, 8, 10, 0, 0); // date-heure de référence fixée par l'énoncé (piège classique)

// ---------------------------------------------------------------------
// Partie B — CRUD et suivi des modifications
// ---------------------------------------------------------------------

Console.WriteLine("=== Partie B ===");

// B.1 — Ajouter
context.Eleves.Add(new Eleve { Nom = "Maxime Petit", Email = "maxime.petit@mail.test", DateInscription = DateTime.Today });
await context.SaveChangesAsync();
Console.WriteLine("B.1 — Maxime Petit inscrit.");

// B.2 — Modifier (aucun appel explicite à Update : le ChangeTracker suit déjà
// cette entité chargée depuis le contexte, SaveChanges détecte seul la propriété modifiée)
var lecon7 = await context.Lecons.FirstAsync(l => l.DureeMinutes == 45 && l.DateHeure == new DateTime(2025, 5, 9, 9, 0, 0));
lecon7.Statut = StatutLecon.Realisee;
await context.SaveChangesAsync();
Console.WriteLine("B.2 — Leçon du 09/05 passée à Realisee.");

// B.3 — Supprimer
var leconAnnulee = await context.Lecons.FirstAsync(l => l.Statut == StatutLecon.Annulee);
context.Lecons.Remove(leconAnnulee);
await context.SaveChangesAsync();
Console.WriteLine("B.3 — Leçon annulée supprimée.");

// B.4 — Gérer une erreur d'intégrité (MoniteurId inexistant)
try
{
    context.Lecons.Add(new Lecon
    {
        DateHeure = DateTime.Today,
        DureeMinutes = 60,
        Statut = StatutLecon.Planifiee,
        MoniteurId = 99,   // n'existe pas
        EleveId = 1
    });
    await context.SaveChangesAsync();
}
catch (DbUpdateException)
{
    Console.WriteLine("B.4 — Échec attendu : le moniteur n°99 n'existe pas (contrainte de clé étrangère).");
    context.ChangeTracker.Clear(); // on retire l'entité invalide du suivi avant de continuer
}

// ---------------------------------------------------------------------
// Partie C — Requêtage LINQ to Entities
// ---------------------------------------------------------------------

Console.WriteLine("\n=== Partie C ===");

// C.1 — Planning d'un moniteur — attendu : leçons #1, 2, 5, 7 (numéros logiques, voir dates), triées par date
var planningNathalie = await context.Lecons
    .Where(l => l.Moniteur.Nom == "Nathalie Moreau")
    .OrderBy(l => l.DateHeure)
    .Select(l => new { l.DateHeure, l.Eleve.Nom, l.Statut })
    .ToListAsync();
Console.WriteLine($"C.1 — {planningNathalie.Count} leçon(s) pour Nathalie Moreau :");
foreach (var x in planningNathalie) Console.WriteLine($"    {x.DateHeure:dd/MM HH:mm} — {x.Nom} — {x.Statut}");

// C.2 — Répartition par statut — attendu : Realisee 4, Planifiee 4, Annulee 0
var parStatut = await context.Lecons
    .GroupBy(l => l.Statut)
    .Select(g => new { Statut = g.Key, Nombre = g.Count() })
    .ToListAsync();
Console.WriteLine("C.2 — Répartition par statut :");
foreach (var x in parStatut) Console.WriteLine($"    {x.Statut} : {x.Nombre}");

// C.3 — Élèves assidus (>= 2 leçons) — attendu : 3 élèves sur 4 (tous sauf Antoine Mercier)
var assidus = await context.Eleves
    .Where(e => e.Lecons.Count >= 2)
    .Select(e => e.Nom)
    .ToListAsync();
Console.WriteLine($"C.3 — Élèves assidus ({assidus.Count}) : {string.Join(", ", assidus)}");

// C.4 — Moniteur le plus sollicité — attendu : Nathalie Moreau, 4 leçons
var moniteurTop = await context.Moniteurs
    .Select(m => new { m.Nom, Nombre = m.Lecons.Count })
    .OrderByDescending(x => x.Nombre)
    .FirstAsync();
Console.WriteLine($"C.4 — Moniteur le plus sollicité : {moniteurTop.Nom} ({moniteurTop.Nombre} leçons)");

// C.5 — Leçons longues (>= 60 min), avec les noms — attendu : 5 leçons
var leconsLongues = await context.Lecons
    .Include(l => l.Moniteur)
    .Include(l => l.Eleve)
    .Where(l => l.DureeMinutes >= 60)
    .Select(l => new { l.DateHeure, Moniteur = l.Moniteur.Nom, Eleve = l.Eleve.Nom })
    .ToListAsync();
Console.WriteLine($"C.5 — Leçons longues ({leconsLongues.Count}) :");
foreach (var x in leconsLongues) Console.WriteLine($"    {x.DateHeure:dd/MM} — {x.Moniteur} / {x.Eleve}");

// C.6 — Prochaines leçons planifiées (2, à partir de la référence incluse) — attendu : 08/05 11h00 puis 10/05 09h00
var prochaines = await context.Lecons
    .Where(l => l.Statut == StatutLecon.Planifiee && l.DateHeure >= reference)
    .OrderBy(l => l.DateHeure)
    .Take(2)
    .Select(l => new { l.DateHeure, l.Eleve.Nom })
    .ToListAsync();
Console.WriteLine("C.6 — Prochaines leçons planifiées :");
foreach (var x in prochaines) Console.WriteLine($"    {x.DateHeure:dd/MM HH:mm} — {x.Nom}");

// ---------------------------------------------------------------------
// Partie D — Stratégies de chargement
// ---------------------------------------------------------------------

Console.WriteLine("\n=== Partie D ===");

// D.1 — Repérez l'erreur
// Code fourni par l'énoncé (fautif) :
//   foreach (var moniteur in context.Moniteurs.ToList())
//       Console.WriteLine($"{moniteur.Nom} : {moniteur.Lecons.Count} leçon(s)");
// Cause : sans Include, la navigation Lecons n'est jamais chargée — elle reste une
// collection vide par défaut (pas d'exception, juste 0 partout). Correction :
Console.WriteLine("D.1 — Nombre de leçons par moniteur (corrigé, avec Include) :");
foreach (var moniteur in await context.Moniteurs.Include(m => m.Lecons).ToListAsync())
    Console.WriteLine($"    {moniteur.Nom} : {moniteur.Lecons.Count} leçon(s)");

// D.2 — Éviter le N+1
// Code fourni par l'énoncé (une requête par élève, dans la boucle) :
//   foreach (var eleve in context.Eleves.ToList())
//   {
//       var nbLecons = context.Lecons.Count(l => l.EleveId == eleve.Id);
//       Console.WriteLine($"{eleve.Nom} : {nbLecons} leçon(s)");
//   }
// Correction : une seule requête SQL, grâce à la navigation Eleve.Lecons
Console.WriteLine("D.2 — Nombre de leçons par élève (corrigé, une seule requête) :");
var comptageParEleve = await context.Eleves
    .Select(e => new { e.Nom, Nombre = e.Lecons.Count })
    .ToListAsync();
foreach (var x in comptageParEleve) Console.WriteLine($"    {x.Nom} : {x.Nombre} leçon(s)");

// ---------------------------------------------------------------------
// Bonus — Transaction explicite (+5 points)
// ---------------------------------------------------------------------

async Task AjouterLeconAvecCompteurAsync(ConduiteFacileContext ctx, int moniteurId, int eleveId, DateTime dateHeure, int dureeMinutes)
{
    using var transaction = await ctx.Database.BeginTransactionAsync();
    try
    {
        var moniteur = await ctx.Moniteurs.FindAsync(moniteurId)
            ?? throw new InvalidOperationException("Moniteur introuvable.");

        ctx.Lecons.Add(new Lecon
        {
            MoniteurId = moniteurId,
            EleveId = eleveId,
            DateHeure = dateHeure,
            DureeMinutes = dureeMinutes,
            Statut = StatutLecon.Planifiee
        });
        moniteur.NombreLeconsDonnees += 1;

        await ctx.SaveChangesAsync();      // une seule écriture, les deux changements sont déjà atomiques ici...
        await transaction.CommitAsync();   // ...la transaction explicite garantit la même atomicité si on les séparait en deux SaveChanges
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}

Console.WriteLine("\n=== Bonus ===");
var julienId = (await context.Moniteurs.FirstAsync(m => m.Nom == "Julien Lefevre")).Id;
var luciId = (await context.Eleves.FirstAsync(e => e.Nom == "Lucie Rousseau")).Id;
await AjouterLeconAvecCompteurAsync(context, julienId, luciId, new DateTime(2025, 5, 12, 9, 0, 0), 60);
var julienFinal = await context.Moniteurs.FirstAsync(m => m.Id == julienId);
Console.WriteLine($"Bonus — {julienFinal.Nom} a maintenant donné {julienFinal.NombreLeconsDonnees} leçon(s) comptabilisée(s).");
