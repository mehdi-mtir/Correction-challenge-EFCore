namespace ConduiteFacile.Demo;

public class Lecon
{
    public int Id { get; set; }
    public DateTime DateHeure { get; set; }
    public int DureeMinutes { get; set; }
    public StatutLecon Statut { get; set; }

    public int MoniteurId { get; set; }
    public Moniteur Moniteur { get; set; } = null!;

    public int EleveId { get; set; }
    public Eleve Eleve { get; set; } = null!;
}
