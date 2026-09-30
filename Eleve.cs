namespace ConduiteFacile.Demo;

public class Eleve
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateInscription { get; set; }

    // Navigation inverse — utilisée en Partie D.2 (e.Lecons.Count, une seule requête SQL)
    public ICollection<Lecon> Lecons { get; set; } = new List<Lecon>();
}
