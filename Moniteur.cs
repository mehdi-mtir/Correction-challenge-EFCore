namespace ConduiteFacile.Demo;

public class Moniteur
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string NumeroPermis { get; set; } = string.Empty;

    // Navigation inverse — utilisée en Partie D (moniteur.Lecons.Count)
    public ICollection<Lecon> Lecons { get; set; } = new List<Lecon>();

    // Ajouté pour le bonus (Partie « transaction ») : mis à jour manuellement
    // dans la même transaction que l'ajout de la leçon correspondante.
    public int NombreLeconsDonnees { get; set; } = 0;
}
